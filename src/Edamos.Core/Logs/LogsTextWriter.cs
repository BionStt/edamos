using System.IO;
using System.Text;

namespace Edamos.Core.Logs
{
    public class LogsTextWriter : TextWriter
    {
        public override Encoding Encoding { get; }        
    }
}