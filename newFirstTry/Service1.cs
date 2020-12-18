using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Xml;
using newFirstTry.CopyingAndArchiving;
using System.Configuration;

namespace newFirstTry
{
    public partial class Service1 : ServiceBase
    {
      
        Logger logger;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.AutoLog = true;
            this.CanPauseAndContinue = true;
        }

        public void StartLogic()
        {
            logger = new Logger(@"C:\Users\User\Desktop\ThirdLab\finaltry\newFirstTry\config.xml");
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStart(string[] args)
        {
            StartLogic();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(5000);
        }
    }
    class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        bool create=false;
        bool rename=false;
        bool archived = false;
        string xmlPath = "";
        string forRenaming = "";
        string startPath = "";
        string archivePath = "";
        string archiveDateTime = "";
        string archiveNameForGZ = "";
        string archiveGZextension = "";
        string recordingPathOfLog = "";
        string recordingDateTime = "";
        

        public Logger(string path)
        {
            xmlPath = path;
            Load_from_xml from_Xml = new Load_from_xml();
            startPath = from_Xml.Load(path, "startPath" );
            archivePath = from_Xml.Load(path, "archivingPath");
            archiveDateTime = from_Xml.Load(path, "archivingDatetime");
            archiveNameForGZ = from_Xml.Load(path, "archivingNameforgzips");
            archiveGZextension = from_Xml.Load(path, "archivingGZextension");
            recordingPathOfLog = from_Xml.Load(path, "recordingPath");
            recordingDateTime = from_Xml.Load(path, "recordingDatetimetooutput");
            watcher = new FileSystemWatcher(startPath);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Renamed += Watcher_Renamed;
            
        }


        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(2000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        public void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string FileEvent = "удален";
            string filePath = e.FullPath;
            Recording(FileEvent, filePath);
        }

        public void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string FileEvent = "создан";
            string filePath = e.FullPath;
            
            Archiving.ArchivingAndCopying(FileEvent, filePath, create, rename, archivePath, archiveDateTime, archiveNameForGZ, archiveGZextension, recordingPathOfLog, recordingDateTime,archived);
            Recording(FileEvent, filePath);
        }

        public void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string FileEvent = "переименован";
            forRenaming = e.FullPath;
            string filePath = e.OldFullPath;
            Archiving.ArchivingAndCopying(FileEvent, forRenaming, create, rename, archivePath, archiveDateTime, archiveNameForGZ, archiveGZextension, recordingPathOfLog, recordingDateTime,archived);
            Recording(FileEvent, filePath);
            
        }

       
        //для записи того что происходит в папке в log
        public void Recording(string FileEvent, string filePath)
        {
            
            
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(recordingPathOfLog,true))
                {
                    writer.WriteLine($"{DateTime.Now.ToString(recordingDateTime)} Файл {filePath} был {FileEvent}");
                    
                    switch (FileEvent)
                    {
                        case "создан":
                            if (create==true)
                            {
                                writer.WriteLine($"Файл {filePath} также был отправлен в архив");
                            }
                            
                            break;
                        case "переименован":
                            writer.WriteLine($" в {forRenaming}");
                            if (rename==true)
                            { 
                                writer.WriteLine($"Файл {forRenaming} также был отправлен в архив");
                            }
                            break;
                        
                    }
                    writer.Flush();
                }
            }
            create = false;
            rename = false;
            archived = false;
        }
    }
}
