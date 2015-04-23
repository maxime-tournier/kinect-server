using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Threading;

namespace Riken
{
    static class Kinect
    {
        public static IEnumerable<BodyFrameReader> DefaultReader()
        {
            var sensor = KinectSensor.GetDefault();
            sensor.Open();
            // don't even wait for sensor to be available yo

            var reader = sensor.BodyFrameSource.OpenReader();
            yield return reader;

            reader.Dispose();
            sensor.Close();
        }

        public static IEnumerable<BodyFrame> BodyFrames()
        {
            foreach (var reader in DefaultReader())
            {
                while (true)
                {
                    yield return reader.AcquireLatestFrame();
                }
            }

        }
        
        public class BodyData {
            public Body[] bodies;
            public long timestamp;
        };

        static public IEnumerable<BodyData> Bodies(IEnumerable<BodyFrame> source)
        {
            Body[] bodies = null;

            foreach (var frame in source)
            {
                
                if (frame == null)
                {
                    yield return null;
                }
                else
                {
                    if (bodies == null)
                    {
                        bodies = new Body[frame.BodyCount];
                    }
                    frame.GetAndRefreshBodyData(bodies);
                    
                    yield return new BodyData() {
                        bodies = bodies, 
                        timestamp = frame.RelativeTime.Ticks
                    };
                }
            }

        }

        static public Body[] Tracked(this Body[] bodies)
        {

            if (bodies == null) return null;
            else
            {
                return bodies.Where((body) => (body != null) && body.IsTracked).ToArray();
            }

        }
    }
}
