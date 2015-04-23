using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Web.Script.Serialization;
using TestServer;


namespace Riken
{
    public class Serializer : JavaScriptSerializer
    {

        protected virtual IEnumerable<object> Positions(Body body)
        {
            foreach (var it in body.Joints)
            {
                var position = it.Key.ToString();
                var x = it.Value.Position.X;
                var y = it.Value.Position.Y;
                var z = it.Value.Position.Z;
                yield return new { position, x, y, z };

            }

        }

        protected virtual IEnumerable<object> HoldState()
        {
            // TODO get grasp state from kinect hands ?
            string hold = "true";
            yield return new { hold };

        }

        protected virtual IEnumerable<object> Timestamp(long timestamp)
        {
            yield return new { timestamp };
        }


        public string Serialize(Body body, long timestamp)
        {
            var frame = Timestamp(timestamp).Concat(Positions(body));
            return Serialize(frame);
        }

    }


   
}
