using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Printer.Test
{
    using Commands;
    using Exceptions;
    using Indicators.Exceptions;
    using Notifiers;

    using Fakes;
    using Stubs;

    [TestClass]
    public class PrintColorCommandTest
    {
        private readonly byte[] _content = System.Text.Encoding.UTF8.GetBytes("Print this!");

        [ExpectedException(typeof(NotEnoughPaperException))]
        [TestMethod]
        public async Task Should_Throw_NotEnoughPaperException_When_Not_Enough_Paper()
        {
            var printColor = new PrintColorCommand(_content,
                paperLevelIndicator: new NotEnoughPaperLevelStub(),
                blackLevelIndicator: new EnoughBlackLevelStub(),
                colorLevelIndicator: new EnoughColorLevelStub());

            await printColor.Execute();
        }

        [ExpectedException(typeof(NotEnoughBlackException))]
        [TestMethod]
        public async Task Should_Throw_NotEnoughBlackException_When_Not_Enough_Black()
        {
            var printColor = new PrintColorCommand(_content,
                paperLevelIndicator: new EnoughPaperLevelStub(),
                blackLevelIndicator: new NotEnoughBlackLevelStub(),
                colorLevelIndicator: new EnoughColorLevelStub());

            await printColor.Execute();
        }

        [ExpectedException(typeof(NotEnoughColorException))]
        [TestMethod]
        public async Task Should_Throw_NotEnoughColorException_When_Not_Enough_Black()
        {
            var printColor = new PrintColorCommand(_content,
                paperLevelIndicator: new EnoughPaperLevelStub(),
                blackLevelIndicator: new EnoughBlackLevelStub(),
                colorLevelIndicator: new NotEnoughColorLevelStub());

            await printColor.Execute();
        }

        [TestMethod]
        public async Task Should_Notify_On_Finish_When_Subscribed_To_Channel_PrinterDotPrintedDotCommandId()
        {
            bool areEqual = false;

            var fn = new FakePrinterNotifier();

            var printColor = new PrintColorCommand(_content,
                paperLevelIndicator: new EnoughPaperLevelStub(),
                blackLevelIndicator: new EnoughBlackLevelStub(),
                colorLevelIndicator: new EnoughColorLevelStub(),
                notifier: fn);

            await fn.Subscribe((message) => areEqual = message.Equals($"Document {printColor.Id} printed"),
            PrinterNotificationTypes.Printed,
            correlationId: printColor.Id);

            await printColor.Execute();

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public async Task Should_Notify_On_Finish_When_Subscribed_To_Channel_PrinterDotPrintedDotCommandId_Only_When_CommandId_Is_Correct()
        {
            Guid incorrectId = Guid.Empty;
            bool areEqual = false;

            var fn = new FakePrinterNotifier();

            var printColor = new PrintColorCommand(_content,
                paperLevelIndicator: new EnoughPaperLevelStub(),
                blackLevelIndicator: new EnoughBlackLevelStub(),
                colorLevelIndicator: new EnoughColorLevelStub(),
                notifier: fn);

            await fn.Subscribe((message) => areEqual = true, PrinterNotificationTypes.Printed);

            await printColor.Execute();

            Assert.IsFalse(areEqual);
        }

        [ExpectedException(typeof(NoContentToPrintException))]
        [TestMethod]
        public async Task Should_Throw_NoContentToPrintException_When_Content_Is_Null()
        {
            var printColor = new PrintColorCommand(
                content: null,
                paperLevelIndicator: new EnoughPaperLevelStub(),
                blackLevelIndicator: new EnoughBlackLevelStub(),
                colorLevelIndicator: new EnoughColorLevelStub());

            await printColor.Execute();
        }

        [ExpectedException(typeof(NoContentToPrintException))]
        [TestMethod]
        public async Task Should_Throw_NoContentToPrintException_When_Content_Is_Empty_Byte_Array()
        {
            var printColor = new PrintColorCommand(
                content: new byte[0],
                paperLevelIndicator: new EnoughPaperLevelStub(),
                blackLevelIndicator: new EnoughBlackLevelStub(),
                colorLevelIndicator: new EnoughColorLevelStub());

            await printColor.Execute();
        }

        [TestMethod]
        public void Should_Throw_InvalidConfigurationException_For_IPaperLevelIndicator_When_Null()
        {
            bool areEqual = false;

            try
            {
                var printColor = new PrintColorCommand(_content,
                    paperLevelIndicator: null,
                    blackLevelIndicator: new EnoughBlackLevelStub(),
                    colorLevelIndicator: new EnoughColorLevelStub());
            }
            catch (InvalidConfigurationException ex)
            {
                areEqual = ex.Message.Equals("There is no implementation for Printer.Indicators.IPaperLevelIndicator");
            }
            finally
            {
                Assert.IsTrue(areEqual);
            }
        }

        [TestMethod]
        public void Should_Throw_InvalidConfigurationException_For_IBlackLevelIndicator_When_Null()
        {
            bool areEqual = false;

            try
            {
                var printColor = new PrintColorCommand(_content,
                    paperLevelIndicator: new EnoughPaperLevelStub(),
                    blackLevelIndicator: null,
                    colorLevelIndicator: new EnoughColorLevelStub());
            }
            catch (InvalidConfigurationException ex)
            {
                areEqual = ex.Message.Equals("There is no implementation for Printer.Indicators.IBlackLevelIndicator");
            }
            finally
            {
                Assert.IsTrue(areEqual);
            }
        }

        [TestMethod]
        public void Should_Throw_InvalidConfigurationException_For_IColorLevelIndicator_When_Null()
        {
            bool areEqual = false;

            try
            {
                var printColor = new PrintColorCommand(_content,
                    paperLevelIndicator: new EnoughPaperLevelStub(),
                    blackLevelIndicator: new EnoughBlackLevelStub(),
                    colorLevelIndicator: null);
            }
            catch (InvalidConfigurationException ex)
            {
                areEqual = ex.Message.Equals("There is no implementation for Printer.Indicators.IColorLevelIndicator");
            }
            finally
            {
                Assert.IsTrue(areEqual);
            }
        }

        [TestMethod]
        public async Task Should_Execute_PrintBlackCommand_Without_Errors_When_Configured_Correctly_And_All_Level_Indicators_Are_Sufficient()
        {
            bool isSuccess = true;

            var printColor = new PrintColorCommand(_content,
                paperLevelIndicator: new EnoughPaperLevelStub(),
                blackLevelIndicator: new EnoughBlackLevelStub(),
                colorLevelIndicator: new EnoughColorLevelStub());

            try
            {
                await printColor.Execute();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            finally
            {
                Assert.IsTrue(isSuccess);
            }
        }
    }
}