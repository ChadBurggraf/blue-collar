//-----------------------------------------------------------------------
// <copyright file="FileSystemWatcher.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    /// <summary>
    /// Wraps a <see cref="System.IO.FileSystemWatcher"/> object to filter and raise
    /// a single event for each path that triggers a rapid series of events.
    /// </summary>
    [SecurityCritical]
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [SuppressMessage("Microsoft.Security", "CA2135:SecurityRuleSetLevel2MethodsShouldNotBeProtectedWithLinkDemandsFxCopRule", Justification = "Conflicting recommendations.")]
    internal sealed class FileSystemWatcher : IDisposable
    {
        #region Private Fields

        private readonly object locker = new object();
        private Thread dequeueThread;
        private System.IO.FileSystemWatcher innerWatcher;
        private Dictionary<string, PathEventItem> pathEvents;
        private bool enableRaisingEvents, disposed;
        private int? threshold;
        
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the FileSystemWatcher class.
        /// </summary>
        public FileSystemWatcher()
        {
            this.innerWatcher = new System.IO.FileSystemWatcher();
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the FileSystemWatcher class.
        /// </summary>
        /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Shared by multiple projects internally.")]
        public FileSystemWatcher(string path)
        {
            this.innerWatcher = new System.IO.FileSystemWatcher(path);
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the FileSystemWatcher class.
        /// </summary>
        /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
        /// <param name="filter">The type of files to watch. For example, "*.txt" watches for changes to all text files.</param>
        public FileSystemWatcher(string path, string filter)
        {
            this.innerWatcher = new System.IO.FileSystemWatcher(path, filter);
            this.Initialize();
        }

        /// <summary>
        /// Finalizes an instance of the FileSystemWatcher class.
        /// </summary>
        ~FileSystemWatcher()
        {
            this.Dispose(false);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is changed.
        /// </summary>
        public event FileSystemEventHandler Changed;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is created.
        /// </summary>
        public event FileSystemEventHandler Created;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is deleted.
        /// </summary>
        public event FileSystemEventHandler Deleted;

        /// <summary>
        /// Occurs when the component is disposed by a call to the <see cref="Dispose()"/> method.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Occurs when the internal buffer overflows.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Occurs whenever any of the <see cref="Changed"/>, <see cref="Created"/>, <see cref="Deleted"/> or <see cref="Renamed"/>
        /// events occur.
        /// </summary>
        public event FileSystemEventHandler Operation;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is renamed.
        /// </summary>
        public event RenamedEventHandler Renamed;

        #endregion

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets a value indicating whether the component is enabled.
        /// </summary>
        public bool EnableRaisingEvents
        {
            get 
            {
                lock (this.locker)
                {
                    return this.enableRaisingEvents;
                }
            }
            
            set 
            {
                bool changed, enabled;

                lock (this.locker)
                {
                    changed = this.enableRaisingEvents != value;
                    this.enableRaisingEvents = enabled = value;
                    this.innerWatcher.EnableRaisingEvents = value;

                    if (!value)
                    {
                        this.pathEvents.Clear();
                    }
                }

                if (changed && enabled)
                {
                    this.dequeueThread = new Thread(this.DequeueEvents);
                    this.dequeueThread.Start();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter string used to determine what files are monitored in a directory.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Shared by multiple projects internally,")]
        public string Filter
        {
            get { return this.innerWatcher.Filter; }
            set { this.innerWatcher.Filter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether subdirectories within the specified path should be monitored.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Shared by multiple projects internally,")]
        public bool IncludeSubdirectories
        {
            get { return this.innerWatcher.IncludeSubdirectories; }
            set { this.innerWatcher.IncludeSubdirectories = value; }
        }

        /// <summary>
        /// Gets or sets the size of the internal buffer.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Shared by multiple projects internally,")]
        public int InternalBufferSize
        {
            get { return this.innerWatcher.InternalBufferSize; }
            set { this.innerWatcher.InternalBufferSize = value; }
        }

        /// <summary>
        /// Gets or sets the mode this instance should operate in.
        /// </summary>
        public FileSystemWatcherMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the type of changes to watch for.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Shared by multiple projects internally,")]
        public NotifyFilters NotifyFilter
        {
            get { return this.innerWatcher.NotifyFilter; }
            set { this.innerWatcher.NotifyFilter = value; }
        }

        /// <summary>
        /// Gets or sets the path of the directory to watch.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Shared by multiple projects internally,")]
        public string Path
        {
            get { return this.innerWatcher.Path; }
            set { this.innerWatcher.Path = value; }
        }

        /// <summary>
        /// Gets or sets the threshold, in milliseconds, that determines the window
        /// in which a file system event can be thought of as a "single" event.
        /// Defaults to 500, so all changes happening with 500 milliseconds of each other count
        /// as a single event.
        /// </summary>
        public int Threshold
        {
            get 
            {
                lock (this.locker)
                {
                    if (this.threshold == null || this.threshold < 0)
                    {
                        this.threshold = 500;
                    }

                    return this.threshold.Value;
                }
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value must be greater than or equal to zero.", "value");
                }

                lock (this.locker)
                {
                    this.threshold = value;
                }
            }
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Releases all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Instance Methods

        /// <summary>
        /// Dequeues all of the events that have elapsed the current <see cref="Threshold"/>.
        /// </summary>
        private void DequeueEvents()
        {
            while (this.EnableRaisingEvents)
            {
                List<string> keys = new List<string>();
                List<PathEventItem> paths = new List<PathEventItem>();

                lock (this.locker)
                {
                    foreach (KeyValuePair<string, PathEventItem> pair in this.pathEvents)
                    {
                        if (DateTime.Now.Subtract(pair.Value.LastRaised).TotalMilliseconds > this.Threshold)
                        {
                            keys.Add(pair.Key);
                            paths.Add(pair.Value);
                        }
                    }

                    foreach (string key in keys)
                    {
                        this.pathEvents.Remove(key);
                    }
                }

                foreach (PathEventItem path in paths)
                {
                    this.RaiseEvent(path);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Releases all resources used by this instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether explicitly disposing.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.EnableRaisingEvents = false;

                if (disposing)
                {
                    if (this.innerWatcher != null)
                    {
                        this.innerWatcher.Dispose();
                        this.innerWatcher = null;
                    }
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Enqueues and possibly throttles an event for raising.
        /// </summary>
        /// <param name="path">The path that raised the original event.</param>
        /// <param name="eventType">The type of the original event that was raised.</param>
        /// <param name="e">The arguments passed when the original event was raised.</param>
        private void EnqueueEvent(string path, FileSystemEventType eventType, FileSystemEventArgs e)
        {
            DateTime now = DateTime.Now;
            string key = this.Mode == FileSystemWatcherMode.Directory ? this.Path : path;

            lock (this.locker)
            {
                if (!this.pathEvents.ContainsKey(key))
                {
                    this.pathEvents[key] = new PathEventItem();
                }

                PathEventItem item = this.pathEvents[key];
                item.PublishEventArgs = e;
                item.PublishEventType = eventType;
                item.LastRaised = now;
                item.RaisedCount++;
            }
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        private void Initialize()
        {
            this.innerWatcher.Changed += new FileSystemEventHandler(this.InnerWatcherChanged);
            this.innerWatcher.Created += new FileSystemEventHandler(this.InnerWatcherCreated);
            this.innerWatcher.Deleted += new FileSystemEventHandler(this.InnerWatcherDeleted);
            this.innerWatcher.Disposed += new EventHandler(this.InnerWatcherDisposed);
            this.innerWatcher.Error += new ErrorEventHandler(this.InnerWatcherError);
            this.innerWatcher.Renamed += new RenamedEventHandler(this.InnerWatcherRenamed);

            this.pathEvents = new Dictionary<string, PathEventItem>();
            this.Mode = FileSystemWatcherMode.IndividualFiles;
        }

        /// <summary>
        /// Raises the innerWatcher's Changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void InnerWatcherChanged(object sender, FileSystemEventArgs e)
        {
            this.EnqueueEvent(e.FullPath, FileSystemEventType.Changed, e);
        }

        /// <summary>
        /// Raises the innerWatcher's Created event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void InnerWatcherCreated(object sender, FileSystemEventArgs e)
        {
            this.EnqueueEvent(e.FullPath, FileSystemEventType.Created, e);
        }

        /// <summary>
        /// Raises the innerWatcher's Deleted event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void InnerWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            this.EnqueueEvent(e.FullPath, FileSystemEventType.Deleted, e);
        }

        /// <summary>
        /// Raises the innerWatcher's Disposed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void InnerWatcherDisposed(object sender, EventArgs e)
        {
            if (this.Disposed != null)
            {
                this.Disposed(this, e);
            }
        }

        /// <summary>
        /// Raises the innerWatcher's Error event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void InnerWatcherError(object sender, ErrorEventArgs e)
        {
            if (this.Error != null)
            {
                this.Error(this, e);
            }
        }

        /// <summary>
        /// Raises the innerWatcher's Renamed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void InnerWatcherRenamed(object sender, RenamedEventArgs e)
        {
            this.EnqueueEvent(e.OldFullPath, FileSystemEventType.Renamed, e);
        }

        /// <summary>
        /// Raises a throttled event to any listeners of this instance.
        /// </summary>
        /// <param name="item">The identifying the event to raise.</param>
        private void RaiseEvent(PathEventItem item)
        {
            switch (item.PublishEventType)
            {
                case FileSystemEventType.Changed:
                    if (this.Changed != null)
                    {
                        this.Changed(this, item.PublishEventArgs);
                    }

                    break;
                case FileSystemEventType.Created:
                    if (this.Created != null)
                    {
                        this.Created(this, item.PublishEventArgs);
                    }

                    break;
                case FileSystemEventType.Deleted:
                    if (this.Deleted != null)
                    {
                        this.Deleted(this, item.PublishEventArgs);
                    }

                    break;
                case FileSystemEventType.Renamed:
                    if (this.Renamed != null)
                    {
                        this.Renamed(this, (RenamedEventArgs)item.PublishEventArgs);
                    }

                    break;
                default:
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture, "Cannot raise event type \"{0}\".", item.PublishEventType),
                        "item");
            }

            if (this.Operation != null)
            {
                this.Operation(this, item.PublishEventArgs);
            }
        }

        #endregion

        #region PathEventItem Class

        /// <summary>
        /// Represents a path and its metadata in relation to <see cref="FileSystemWatcher"/> events.
        /// </summary>
        [Serializable]
        private class PathEventItem : ISerializable
        {
            private DateTime lastRaised;
            private FileSystemEventArgs publishEventArgs;
            private FileSystemEventType publishEventType;
            private int raisedCount;

            /// <summary>
            /// Initializes a new instance of the PathEventItem class.
            /// </summary>
            public PathEventItem()
            {
            }

            /// <summary>
            /// Initializes a new instance of the PathEventItem class.
            /// </summary>
            /// <param name="info">The <see cref="SerializationInfo"/> to load data from..</param>
            /// <param name="context">The source <see cref="StreamingContext"/> for this serialization.</param>
            public PathEventItem(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info", "info cannot be null.");
                }

                this.publishEventArgs = (FileSystemEventArgs)info.GetValue("publishEventArgs", typeof(FileSystemEventArgs));
                this.publishEventType = (FileSystemEventType)Enum.ToObject(typeof(FileSystemEventType), info.GetInt32("publishEventType"));
                this.lastRaised = info.GetDateTime("lastRaised");
                this.raisedCount = info.GetInt32("raisedCount");
            }

            /// <summary>
            /// Gets or sets the date the last event for this path was raised.
            /// </summary>
            public DateTime LastRaised
            {
                get { return this.lastRaised; }
                set { this.lastRaised = value; }
            }

            /// <summary>
            /// Gets or sets the arguments to raise the event to publish with.
            /// </summary>
            public FileSystemEventArgs PublishEventArgs
            {
                get { return this.publishEventArgs; }
                set { this.publishEventArgs = value; }
            }

            /// <summary>
            /// Gets or sets the type of event to publish.
            /// </summary>
            public FileSystemEventType PublishEventType
            {
                get { return this.publishEventType; }
                set { this.publishEventType = value; }
            }

            /// <summary>
            /// Gets or sets the number of times a file system event has been raised for this path.
            /// </summary>
            public int RaisedCount
            {
                get { return this.raisedCount; }
                set { this.raisedCount = value; }
            }

            /// <summary>
            /// Populates a <see cref="SerializationInfo"/>  with the data needed to serialize the target object.
            /// </summary>
            /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
            /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info", "info cannot be null.");
                }

                info.AddValue("publishEventArgs", this.publishEventArgs);
                info.AddValue("publishEventType", this.publishEventType);
                info.AddValue("lastRaised", this.lastRaised);
                info.AddValue("raisedCount", this.raisedCount);
            }
        }

        #endregion
    }
}
