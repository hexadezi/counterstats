using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;

/// <summary>
/// A library to monitor the appended data to a file like tail -f.
/// </summary>
public class TailNET
{
	#region Fields

	private readonly FileInfo file;
	private readonly string delimiter = Environment.NewLine;
	private readonly System.Timers.Timer timer = new System.Timers.Timer() { Interval = 500 };
	private readonly Encoding encoding = Encoding.UTF8;
	private readonly object processingLock = new object();
	private readonly StringBuilder buffer = new StringBuilder();
	private long oldSize = -1;

	#endregion Fields

	#region Properties

	/// <summary>
	/// Determines whether the file size is reset before restarting.
	/// If true, all changes between stop and start are discarded.
	/// </summary>
	public bool ResetBeforeRestart { get; set; } = true;

	#endregion Properties

	#region Events

	/// <summary>
	/// Occurs when a new line is added to file.
	/// </summary>
	public event EventHandler<string> LineAdded;

	/// <summary>
	/// Occurs when the file is deleted
	/// </summary>
	public event EventHandler FileDeleted;

	/// <summary>
	/// Occurs when monitoring starts
	/// </summary>
	public event EventHandler Started;

	/// <summary>
	/// Occurs when monitoring stops
	/// </summary>
	public event EventHandler Stopped;

	#endregion Events

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="TailNET"/> class.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	public TailNET(string filePath)
	{
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException($"Could not find file {filePath}");
		}

		file = new FileInfo(filePath);

		timer.Elapsed += Tick;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TailNET"/> class.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="interval">The interval. If not specified, 500 ms is defined as default.</param>
	public TailNET(string filePath, int interval) : this(filePath, Environment.NewLine, interval)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TailNET"/> class.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="encoding">The encoding. If not specified, UTF8 is defined as default.</param>
	public TailNET(string filePath, Encoding encoding) : this(filePath, Environment.NewLine, 500, encoding)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TailNET"/> class.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="delimiter">The delimiter. If not specified, Environment.NewLine is defined as default.</param>
	public TailNET(string filePath, string delimiter) : this(filePath)
	{
		// Delimiter can not be null or empty. It would throw an exception  while processing.
		if (delimiter is null || delimiter is "")
		{
			throw new ArgumentException("No null or empty string allowed");
		}

		this.delimiter = delimiter;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TailNET"/> class.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="delimiter">The delimiter. If not specified, Environment.NewLine is defined as default.</param>
	/// <param name="interval">The interval. If not specified, 500 ms is defined as default.</param>
	public TailNET(string filePath, string delimiter, int interval) : this(filePath, delimiter)
	{
		timer.Interval = interval;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TailNET"/> class.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="delimiter">The delimiter. If not specified, Environment.NewLine is defined as default.</param>
	/// <param name="interval">The interval. If not specified, 500 ms is defined as default.</param>
	/// <param name="encoding">The encoding. If not specified, UTF8 is defined as default.</param>
	public TailNET(string filePath, string delimiter, int interval, Encoding encoding) : this(filePath, delimiter, interval)
	{
		this.encoding = encoding;
	}

	#endregion Constructors

	#region Methods

	/// <summary>
	/// The method that is called when the timer ticks.
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The timer event args.</param>
	private void Tick(object sender, ElapsedEventArgs e)
	{
		if (!File.Exists(file.FullName))
		{
			Stop();
			FireWithTry(FileDeleted);
			Debug.WriteLine("File deleted");
			return;
		}

		// Return if lock can not be aquired
		if (!Monitor.TryEnter(processingLock))
		{
			return;
		}
		// When the lock is obtained, start processing
		try
		{
			// If still initial
			if (oldSize == -1)
			{
				oldSize = file.Length;
				Debug.WriteLine("Initial file size set to " + oldSize);
			}

			// The current file size is needed
			long newSize = new FileInfo(file.FullName).Length;

			// If old size and current size are the same, we do not need further processing
			if (oldSize == newSize)
			{
				return;
			}

			// If the current file size is smaller than the old size, the file has been emptied
			// The old size will be set to the current smaller size and the buffer will be emptied
			if (oldSize > newSize)
			{
				Debug.WriteLine($"File size has decreased. Reset file size to {newSize}");

				oldSize = newSize;

				Debug.WriteLine("Clear buffer");

				buffer.Clear();

				return;
			}

			Debug.WriteLine($"Old size {oldSize,9} | New size {newSize,9}");

			using FileStream fileStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using StreamReader sr = new StreamReader(fileStream, encoding);
			sr.BaseStream.Seek(oldSize, SeekOrigin.Begin);

			while (!sr.EndOfStream)
			{
				buffer.Append((Char)sr.Read());

				// If the delimiter is bigger, we don't have to do anything.
				if (delimiter.Length > buffer.Length)
				{
					continue;
				}

				// We only check if the last char of the delimiter and buffer.
				// If they are not equal, no further processing is needed.
				// This way the performance can be improved drastically.
				// If delimiter is null or empty, it will throw an exception.
				if (delimiter[^1] != buffer[^1])
				{
					continue;
				}

				if (!buffer.ToString().EndsWith(delimiter, StringComparison.Ordinal))
				{
					continue;
				}

				buffer.Remove(buffer.Length - delimiter.Length, delimiter.Length);

				FireWithTry(LineAdded, buffer.ToString());

				buffer.Clear();
			}

			oldSize = newSize;
		}
		finally
		{
			// Ensure that the lock is released.
			Monitor.Exit(processingLock);
		}
	}

	/// <summary>
	/// A method, which will invoke each delegate.
	/// Exceptions will be catched and the remaining
	/// delegates will be invoked.
	/// </summary>
	/// <param name="eventHandler">A predefined delegate that represents an event handler method for an event.</param>
	public void FireWithTry(EventHandler eventHandler)
	{
		if (eventHandler != null)
		{
			//We iterate through the invocation list and invoke each delegate separately
			//If an exception occurs, the other delegates are still called.
			foreach (EventHandler handler in eventHandler.GetInvocationList())
			{
				try
				{
					handler(this, EventArgs.Empty);
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Error in handler {handler.Method.Name} in class {handler.Method.DeclaringType}: {ex.Message}");
					Trace.WriteLine($"Error in handler {handler.Method.Name} in class {handler.Method.DeclaringType}: {ex.Message}");
				}
			}
		}
	}

	/// <summary>
	/// A method, which will invoke each delegate.
	/// Exceptions will be catched and the remaining
	/// delegates will be invoked.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="eventHandler">A predefined delegate that represents an event handler method for an event.</param>
	/// <param name="e">An object that contains the event data.</param>
	public void FireWithTry<T>(EventHandler<T> eventHandler, T e)
	{
		if (eventHandler is null)
		{
			return;
		}

		//We iterate through the invocation list and invoke each delegate separately
		//If an exception occurs, the other delegates are still called.
		foreach (EventHandler<T> handler in eventHandler.GetInvocationList())
		{
			try
			{
				handler(this, e);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error in handler {handler.Method.Name} in class {handler.Method.DeclaringType}: {ex.Message}");
				Trace.WriteLine($"Error in handler {handler.Method.Name} in class {handler.Method.DeclaringType}: {ex.Message}");
			}
		}
	}

	/// <summary>
	/// Starts the file monitoring.
	/// </summary>
	public void Start()
	{
		if (!File.Exists(file.FullName))
		{
			Debug.WriteLine($"File {file.FullName} not found. Returning.");
			Trace.WriteLine($"File {file.FullName} not found. Returning.");
			return;
		}

		if (timer.Enabled)
		{
			return;
		}

		if (ResetBeforeRestart && oldSize != -1)
		{
			// The state of the file object has to be refreshed, 
			// to get the current file size
			file.Refresh();
			oldSize = file.Length;
			Debug.WriteLine("Reset file size to " + oldSize);
		}

		timer.Start();
		FireWithTry(Started);
		Debug.WriteLine("Monitoring started");
	}

	/// <summary>
	/// Stops the file monitoring.
	/// </summary>
	public void Stop()
	{
		if (!timer.Enabled)
		{
			return;
		}

		timer.Stop();
		FireWithTry(Stopped);
		Debug.WriteLine("Monitoring stopped");
	}

	#endregion Methods
}