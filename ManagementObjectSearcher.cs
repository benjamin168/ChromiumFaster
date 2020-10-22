using System;

namespace Chrome
{
    internal class ManagementObjectSearcher
    {
        private string wmiQuery;

        public ManagementObjectSearcher(string wmiQuery)
        {
            this.wmiQuery = wmiQuery;
        }

        internal object Get()
        {
            throw new NotImplementedException();
        }
    }
}