using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Framework.Lists;

namespace osu.Game.Rulesets.Mania.Communication
{
    public class EventStart
    {
        public DateTime startedDate;
        public int columns;

       public EventStart(int columns)
        {
            this.columns = columns;
            startedDate = DateTime.Now;
        }
    }
    public class EventStop
    {
        public DateTime stoppedDate;
        public bool hasFailed;
        public bool hasCompleted;
        public EventStop(bool hasFailed, bool hasCompleted)
        {
            stoppedDate = DateTime.Now;
            this.hasFailed = hasFailed;
            this.hasCompleted = hasCompleted;
        }
    }
    public class EventNote
    {
        public int column;
        public int size;
        public double distance;

        public EventNote(int column, int size, double distance)
        {
            this.column = column;
            this.size = size;
            this.distance = distance;
        }
    }
    public class EventBuilder
    {
       public  static DataStructure<EventStart> createStartEvent(int columns)
        {
            return new DataStructure<EventStart>("start", new EventStart(columns));
        }
       public static DataStructure<EventStop> createStopEvent(bool hasFailed = false, bool hasCompleted = false)
        {
            return new DataStructure<EventStop>("stop", new EventStop(hasFailed, hasCompleted));
        }
      public  static DataStructure<EventNote> createNoteUpdateEvent(int column, int size, double distance) {
            return new DataStructure<EventNote>("updateNote", new EventNote(column, size, distance));
        }

    }
    public class SocketCommunication
    {
        private UdpClient socket;
        public bool isClosed { get; set; }
        public UdpClient Socket
        {
            get { return socket; }
            set { socket = value; }
        }
        public Dictionary<int, ColumnHitObject> Columns = new Dictionary<int, ColumnHitObject>();
        private string ip = "127.0.0.1";
        private int port = 6616;
        public SocketCommunication()
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new UdpClient();
            socket.Connect(ip, port);
        }
        public void send(String data)
        {
            if (socket != null && !isClosed)
            {
                {
                    var bytes = Encoding.UTF8.GetBytes(data);
                    var dataLenght = Encoding.UTF8.GetByteCount(data.ToString());
                    socket.Send(bytes, dataLenght);
                }
            }
        }
        public void Close()
        {
            if (!isClosed)
            {
                socket.Close();
                isClosed = true;
            }
        }
        public class ColumnHitObject
        {
            public SortedList<ManiaHitObject> HitObjects = new SortedList<ManiaHitObject>((ManiaHitObject a, ManiaHitObject b) =>
            {
                if (a.StartTime > b.StartTime)
                    return 1;
                else
                    return -1;
            }
            );
        }
    }
}
