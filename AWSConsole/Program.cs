using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWS;

namespace AWSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            AWSS3Storage s3 = new AWSS3Storage();
            s3.StartAmazonMain();
            Console.ReadLine();

        }
    }
}
