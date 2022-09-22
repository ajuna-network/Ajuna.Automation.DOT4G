using Ajuna.NetApi.Model.AjunaWorker;
using NUnit.Framework;
using System.Linq;
using System.Text;

namespace Ajuna.Automation.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var hexString = "0x710169014f7468657228224572726f72207b2063617573653a204e6f6e652c20646573633a205c22436f756c64206e6f74206465636f64652060476574746572602c2076617269616e7420646f65736e27742065786973745c22207d22290002";


            var returnValue = new RpcReturnValue();
            returnValue.Create(hexString);


            Assert.That(returnValue.DoWatch.Value, Is.EqualTo(false));
            Assert.That(returnValue.DirectRequestStatus.Value, Is.EqualTo(DirectRequestStatus.Error));

            var errorMsg = System.Text.Encoding.ASCII.GetString(returnValue.Value.Value.ToList().Select(p => p.Value).ToArray());
            Assert.That(errorMsg, Is.EqualTo(""));
        }
    }
}