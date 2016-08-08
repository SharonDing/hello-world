using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

using NUnit.Framework;
using Philips.FlexUnit;

using LCP.Util;
using Cmrg1.WrapperDevice;
using IOLab.Dali;

namespace Cmrg1
{
    //[TestFixture, Category("Cmrg1")]
    [TestFixture, Category("Cmrg1_BasicTest")]
    public class Cmrg1_BasicTest_TestCases : Cmrg1_BasicTest_Setup
    {
        // LOCAL TYPES  =====================================================
        private const int MAX_TIMEOUT = 3000; //ms

        // PUBLIC MEMBER FUNCTIONS ==========================================
        /// <summary>
        /// Get/set the wrapper to communicate with the testharness
        /// </summary>

        // LOCAL FUNCTIONS ==================================================
        private void DrySwitch(UInt16 dry)
        {
            string dryn;

            for (int bit = 0; bit < 4; bit++)
            {
                dryn = string.Concat("Dry", (bit + 1).ToString());
                if ((dry & (1 << bit)) != 0)
                {
                    Flex.Log(string.Concat("Close Dry", (bit + 1).ToString()));
                    target.Relay.Close(dryn);
                }
                else
                {
                    Flex.Log(string.Concat("Open Dry", (bit + 1).ToString()));
                    target.Relay.Open(dryn);
                }
            }
        }

        private UInt16 Dry2Cmd(UInt16 dryC)
        {
            //dry= 0   1  2  3  4  5  6   7   8  9  10 11 12 13 14 15
            UInt16[] DryTable = { 255, 4, 5, 6, 7, 8, 10, 15, 3, 3, 3, 3, 3, 3, 3, 3 };

            Assert.GreaterOrEqual(dryC, 0);
            Assert.LessOrEqual(dryC, 15);
            return DryTable[dryC];
        }

        private UInt16 Cmd2Dry(UInt16 cmd)
        {
            //cmd=  0    1    2   3  4  5  6  7  8  9    10  11   12   13   14  15
            UInt16[] CmdTable = { 255, 255, 255, 8, 1, 2, 3, 4, 5, 255, 6, 255, 255, 255, 255, 7 };

            Assert.GreaterOrEqual(cmd, 0);
            Assert.LessOrEqual(cmd, 15);
            return CmdTable[cmd];
        }

        [SetUp]
        // Summary:
        //     Attribute used to identify a method that is called before any tests in a
        //     fixture are run.
        public new void SetUp()
        {
            Flex.Log("#####################################   SetUp   ###################################");
            //TBD
            // 
            history.Clear();
            //history.RestartRecording();
        }

        [TearDown]
        // Summary:
        //     Attribute used to identify a method that is called after all the tests in
        //     a fixture have run. The method is guaranteed to be called, even if an exception
        //     is thrown.
        public new void TearDown()
        {
            Flex.Log("###################################   TearDown   ##################################");
            //TBD
            history.StopRecording();
        }

        // TESTCASES ========================================================
        #region Cmrg1_1_0_C1_MainsFreqVolt_108v57Hz_L2N
        [Test, Description(@"
        Narrative: Rx can support the US application: Mains voltage=108v, and Mains frequency=57Hz. And Rx should correctly receive the 10bit Coded commands.
 
        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=108v, AC1Freq=57Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=57Hz*3, to AC2 power supply; 
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C1_MainsFreqVolt_108v57Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 108, 57);
            target.AcPower.SetOutput("AC2", 5, 57 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C2_MainsFreqVolt_108v63Hz_L2N
        [Test, Description(@"
        Narrative: Rx can support the US application: Mains voltage=108v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.
 
        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=108v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C2_MainsFreqVolt_108v63Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 108, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C3_MainsFreqVolt_305v57Hz_L2N
        [Test, Description(@"
        Narrative: Rx can support the US application: Mains voltage=305v, and Mains frequency=57Hz. And Rx should correctly receive the 10bit Coded commands.
 
        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=305v, AC1Freq=57Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=57Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C3_MainsFreqVolt_305v57Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 305, 57);
            target.AcPower.SetOutput("AC2", 5, 57 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C4_MainsFreqVolt_305v63Hz_L2N
        [Test, Description(@"
        Narrative: Rx can support the US application: Mains voltage=305v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.
 
        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=305v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C4_MainsFreqVolt_305v63Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 305, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C5_MainsFreqVolt_120v60Hz_L2N
        [Test, Description(@"
        Narrative: Rx can support the US application: Mains voltage=120v, and Mains frequency=60Hz. And Rx should correctly receive the 10bit Coded commands.
 
        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=120v, AC1Freq=60Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=60Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C5_MainsFreqVolt_120v60Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 120, 60);
            target.AcPower.SetOutput("AC2", 5, 60 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C6_MainsFreqVolt_277v60Hz_L2N
        [Test, Description(@"
        Narrative: Rx can support the US application: Mains voltage=277v, and Mains frequency=60Hz. And Rx should correctly receive the 10bit Coded commands.
 
        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=277v, AC1Freq=60Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=60Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C6_MainsFreqVolt_277v60Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 277, 60);
            target.AcPower.SetOutput("AC2", 5, 60 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C7_MainsFreqVolt_198v47Hz_L2N
        [Test, Description(@"
        Narrative: Rx can support the EU application: Mains voltage=198v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.
 
        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=198v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C7_MainsFreqVolt_198v47Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 198, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C8_MainsFreqVolt_198v53Hz_L2N
        [Test, Description(@"
        Narrative:  Rx can support the EU application: Mains voltage=198v, and Mains frequency=53Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=198v, AC1Freq=53Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=53Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C8_MainsFreqVolt_198v53Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 198, 53);
            target.AcPower.SetOutput("AC2", 5, 53 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C9_MainsFreqVolt_264v47Hz_L2N
        [Test, Description(@"
        Narrative:  Rx can support the EU application: Mains voltage=264v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=264v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C9_MainsFreqVolt_264v47Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 264, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C10_MainsFreqVolt_264v53Hz_L2N
        [Test, Description(@"
        Narrative:  Rx can support the EU application: Mains voltage=264v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=264v, AC1Freq=53Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=53Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C10_MainsFreqVolt_264v53Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 264, 53);
            target.AcPower.SetOutput("AC2", 5, 53 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C11_MainsFreqVolt_230v50Hz_L2N
        [Test, Description(@"
        Narrative:  Rx can support the EU application: Mains voltage=230v, and Mains frequency=50Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C11_MainsFreqVolt_230v50Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C12_MainsFreqVolt_108v47Hz_L2N
        [Test, Description(@"
        Narrative:   Rx can support the Boundary application: Mains voltage=108v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=108v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C12_MainsFreqVolt_108v47Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 108, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C13_MainsFreqVolt_108v63Hz_L2N
        [Test, Description(@"
        Narrative:  Rx can support the Boundary application: Mains voltage=108v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=108v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C13_MainsFreqVolt_108v63Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 108, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C14_MainsFreqVolt_305v47Hz_L2N
        [Test, Description(@"
        Narrative:Rx can support the Boundary application: Mains voltage=305v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=305v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C14_MainsFreqVolt_305v47Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 305, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C15_MainsFreqVolt_305v63Hz_L2N
        [Test, Description(@"
        Narrative:Rx can support the Boundary application: Mains voltage=305v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.

        Implement :
        Set all relay connections switch to L-N setting.
        Set AC1Volt=305v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply;  
        Sleep 30s
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        Sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C15_MainsFreqVolt_305v63Hz_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 305, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C16_MainsFreqVolt_Change1_L2N
        [Test, Description(@"
        Narrative:This test case is to verify Rx can correctly decode the commands when Mains frequency or voltage change happened.

        Implement :
        Set all relay connection switch to L-N setting.
        RXAPP_Start

        Set AC1Volt=120v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 5 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;

        Set AC1Volt=120v, AC1Freq=53Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=53Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 6 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;

        Set AC1Volt=130v, AC1Freq=53Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=53Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 7 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C16_MainsFreqVolt_Change1_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int freq = 0, volt = 0;
            int sleepSec = 30 * 1000;

            target.Rxapp.Start();

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (ushort cmd = 5; cmd < 8; cmd++)
            {
                switch (freq)
                {
                    case 0:
                        freq = 47;
                        volt = 120;
                        break;
                    case 47:
                        freq = 53;
                        break;
                    case 53:
                        volt = 130;
                        break;
                }
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, freq);
                target.AcPower.SetOutput("AC2", 5, freq * 3);
                Thread.Sleep(sleepSec);

                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
            }

            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C17_MainsFreqVolt_Change2_L2N
        [Test, Description(@"
        Narrative:This test case is to verify Rx can correctly decode the commands when Mains frequency or voltage change happened.

        Implement :
        Set all relay connection switch to L-N setting.
        RXAPP_Start

        Set AC1Volt=277v, AC1Freq=53Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=53Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 5 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;

        Set AC1Volt=277v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 6 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;

        Set AC1Volt=270v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 7 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C17_MainsFreqVolt_Change2_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int freq = 0, volt = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            target.Rxapp.Start();

            for (ushort cmd = 5; cmd < 8; cmd++)
            {
                switch (freq)
                {
                    case 0:
                        freq = 53;
                        volt = 277;
                        break;
                    case 53:
                        freq = 47;
                        break;
                    case 47:
                        volt = 270;
                        break;
                }
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, freq);
                target.AcPower.SetOutput("AC2", 5, freq * 3);
                Thread.Sleep(sleepSec);

                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
            }

            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C18_MainsFreqVolt_Change3_L2N
        [Test, Description(@"
        Narrative: This test case is to verify Rx can correctly decode the commands when Mains frequency or voltage change happened.

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start
        Set AC1Volt=230v, AC1Freq=57Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=57Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 5 from Tx to Rx 
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
      
        Set AC1Volt=230v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 30s
        Send scene 6 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;

        Set AC1Volt=220v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply;
        Sleep 30s 
        Send scene 7 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C18_MainsFreqVolt_Change3_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int freq = 0, volt = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            target.Rxapp.Start();

            for (ushort cmd = 5; cmd < 8; cmd++)
            {
                switch (freq)
                {
                    case 0:
                        freq = 57;
                        volt = 230;
                        break;
                    case 57:
                        freq = 63;
                        break;
                    case 63:
                        volt = 220;
                        break;
                }
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, freq);
                target.AcPower.SetOutput("AC2", 5, freq * 3);
                Thread.Sleep(sleepSec);

                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
            }

            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C19_SweepFreq_Asc_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains frequency or voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(fFreq=47Hz;fFreq<=63Hz;fFreq+0.1Hz)
        {
          Set AC1Volt=230v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 30s;

          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=8;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=15;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 10s)
          Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C19_SweepFreq_Asc_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (double freq = 470; freq <= 630; freq++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 230, freq / 10);
                target.AcPower.SetOutput("AC2", 0, freq / 10);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Flex.Log("Received CMD is " + cmdRcv);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C20_SweepFreq_Des_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains frequency or voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(fFreq=63Hz;fFreq<=47Hz;fFreq-0.1Hz)
        {
          Set AC1Volt=277v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 30s;

          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=8;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=15;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 10s)
          Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C20_SweepFreq_Des_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (double freq = 630; freq <= 470; freq--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 277, freq / 10);
                target.AcPower.SetOutput("AC2", 0, freq / 10);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C21_FreqJump_Asc_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(fFreq=47Hz;fFreq<=63Hz;fFreq+3Hz)
        {
          Set AC1Volt=277v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 30s;

          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=8;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=15;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 10s)
          Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C21_FreqJump_Asc_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (int freq = 47; freq <= 63; freq++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 277, freq);
                target.AcPower.SetOutput("AC2", 0, freq);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                freq = freq + 2;
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C22_FreqJump_Des_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains frequency or voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(fFreq=63Hz;fFreq<=47Hz;fFreq-3Hz)
        {
          Set AC1Volt=230v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 30s;

          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=8;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=15;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 10s)
          Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C22_FreqJump_Des_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (int freq = 63; freq <= 47; freq--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 230, freq);
                target.AcPower.SetOutput("AC2", 0, freq);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                freq = freq - 2;
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C23_SweepVolt_Asc_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains frequency or voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(Volt=108v; volt<=305v; volt++)
        {
            Set AC1Volt=volt, AC1Freq=60Hz to AC1 power supply; 
            Set AC2Volt=0v, AC2Freq=60Hz  to AC2 power supply;  
            Wait 30s;

             If latest CY1CMD!=8,
             {
                 Send CY1CMD8 from Tx to Rx ;
                 CY1CMD=8;
              }
              Else
              {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=15;
              }
              Wait feedback of Rx wrapper while Rx received this command(time out 10s)
              Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C23_SweepVolt_Asc_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (int volt = 108; volt <= 305; volt++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 60);
                target.AcPower.SetOutput("AC2", 0, 60);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C24_SweepVolt_Des_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains frequency or voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(volt=305v ;fvolt <= 108v;  volt--)
        {
             Set AC1Volt=volt, AC1Freq=50Hz to AC1 power supply; 
             Set AC2Volt=0v, AC2Freq=50Hz to AC2 power supply;  
             Wait 30s;

             If latest CY1CMD!=8,
             {
                 Send CY1CMD8 from Tx to Rx ;
                 CY1CMD=8;
              }
             Else
             {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=15;
              }
              Wait feedback of Rx wrapper while Rx received this command(time out 10s)
              Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C24_SweepVolt_Des_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (int volt = 305; volt <= 108; volt--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 50);
                target.AcPower.SetOutput("AC2", 0, 50);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C25_VoltJump_Asc_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(volt = 108v ; volt <= 305v; volt +5v)
        {
            Set AC1Volt=volt, AC1Freq=60Hz  to AC1 power supply; 
            Set AC2Volt=0v, AC2Freq=60Hz to AC2 power supply;  
            Wait 30s;

            If latest CY1CMD!=8,
            {
                 Send CY1CMD8 from Tx to Rx ;
                 CY1CMD=8;
            }
            Else
            {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=15;
            }
            Wait feedback of Rx wrapper while Rx received this command(time out 10s)
            Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C25_VoltJump_Asc_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (int volt = 108; volt <= 305; volt++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 60);
                target.AcPower.SetOutput("AC2", 0, 60);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                volt = volt + 4;
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C26_VoltJump_Des_L2N
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains frequency or voltage changed

        Implement :
        Set all relay connection switch to L-N setting.
        
        RXAPP_Start;
        For(volt = 305v ; volt <=108v ; volt-5v)
        {
            Set AC1Volt=volt, AC1Freq=50Hz to AC1 power supply; 
            Set AC2Volt=0v, AC2Freq=50Hz to AC2 power supply;  
            Wait 30s;

            If latest CY1CMD!=8,
            {
                Send CY1CMD8 from Tx to Rx ;
                CY1CMD=8;
             }
             Else
             {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=15;
             }
             Wait feedback of Rx wrapper while Rx received this command(time out 10s)
             Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_1_0_C26_VoltJump_Des_L2N()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (int volt = 108; volt <= 305; volt--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 50);
                target.AcPower.SetOutput("AC2", 0, 50);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                volt = volt - 5;
                target.Rxapp.Stop();
            }
        }
        #endregion

        //----------------------new have not been tested------------------
        #region Cmrg1_1_0_C27_MainsFreqVolt_342v47Hz_L2L
        [Test, Description(@"
         Narrative: Rx can support the Boundary application: Mains voltage=342v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands. (stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=342v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C27_MainsFreqVolt_342v47Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 342, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C28_MainsFreqVolt_342v63Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=342v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=342v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C28_MainsFreqVolt_342v63Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 342, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C29_MainsFreqVolt_380v47Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=380v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=380v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C29_MainsFreqVolt_380v47Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 380, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C30_MainsFreqVolt_380v63Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=380v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=380v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C30_MainsFreqVolt_380v63Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 380, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C31_MainsFreqVolt_400v50Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=400v, and Mains frequency=50Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=400v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C31_MainsFreqVolt_400v50Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 400, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C32_MainsFreqVolt_480v60Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=480v, and Mains frequency=60Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=480v, AC1Freq=60Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=60Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C32_MainsFreqVolt_480v60Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 480, 60);
            target.AcPower.SetOutput("AC2", 5, 60 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C33_MainsFreqVolt_480v47Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=480v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=480v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C33_MainsFreqVolt_480v47Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 480, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C34_MainsFreqVolt_480v63Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=480v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=480v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C34_MainsFreqVolt_480v63Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 480, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C35_MainsFreqVolt_528v47Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=528v, and Mains frequency=47Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=528v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C35_MainsFreqVolt_528v47Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 528, 47);
            target.AcPower.SetOutput("AC2", 5, 47 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C36_MainsFreqVolt_528v63Hz_L2L
        [Test, Description(@"
        Narrative: Rx can support the Boundary application: Mains voltage=528v, and Mains frequency=63Hz. And Rx should correctly receive the 10bit Coded commands.(stepdown transformer is needed for this test)
 
        Implement :
        Set all relay connection switch to L-L setting.
        Set AC1Volt=528v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send multiple scene commands from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received these command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s for each switch
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C36_MainsFreqVolt_528v63Hz_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 30 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 528, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_1_0_C37_MainsFreqVolt_Change1_L2L
        [Test, Description(@"
        Narrative: This test case is to verify Rx can correctly decode the commands when Mains frequency or voltage change happened.
 
        Implement :
        Set all relay connection switch to L-L setting.
        RXAPP_Start

        Set AC1Volt=398v, AC1Freq=47Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=47Hz*3, to AC2 power supply; 
        Sleep 20s
        Send scene 5 from Tx to Rx by SCIP.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s
 
        Set AC1Volt=398v, AC1Freq=53Hz, to AC1 power supply; 
        Set AC2Volt=25v, AC2Freq=53Hz*3, to AC2 power supply; 
        Sleep 20s
        Send scene 6 from Tx to Rx by SCIP.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s
 
        Set AC1Volt=390v, AC1Freq=53Hz, to AC1 power supply; 
        Set AC2Volt=25v, AC2Freq=53Hz*3, to AC2 power supply;
        Sleep 20s 
        Send scene 7 from Tx to Rx by SCIP.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s

        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C37_MainsFreqVolt_Change1_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int volt_1 = 0, volt_2 = 0, freq = 0;
            int sleepSec = 20 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            for (ushort cmd = 5; cmd < 8; cmd++)
            {
                switch (cmd)
                {
                    case 5:
                        freq = 47;
                        volt_1 = 398;
                        volt_2 = 5;
                        break;
                    case 6:
                        freq = 53;
                        volt_2 = 25;
                        break;
                    case 7:
                        volt_1 = 390;
                        break;
                }

                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt_1, freq);
                target.AcPower.SetOutput("AC2", volt_2, freq * 3);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5000);
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C38_MainsFreqVolt_Change2_L2L
        [Test, Description(@"
        Narrative: This test case is to verify Rx can correctly decode the commands when Mains frequency or voltage change happened.
 
        Implement :
        Set all relay connection switch to L-L setting.
        RXAPP_Start

        Set AC1Volt=480v, AC1Freq=57Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=57Hz*3, to AC2 power supply; 
        Sleep 20s
        Send scene 5 from Tx to Rx by SCIP.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s
 
        Set AC1Volt=480v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=25v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 20s
        Send scene 6 from Tx to Rx by SCIP.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s
 
        Set AC1Volt=470v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=25v, AC2Freq=63Hz*3, to AC2 power supply;
        Sleep 20s 
        Send scene 7 from Tx to Rx by SCIP.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received 4 bit command equal to sending command;
        sleep 5s

        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C38_MainsFreqVolt_Change2_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int volt_1 = 0, volt_2 = 0, freq = 0;
            int sleepSec = 20 * 1000;


            //switch to L-L
            target.Relay.Close("Dry5");

            for (ushort cmd = 5; cmd < 8; cmd++)
            {
                switch (cmd)
                {
                    case 5:
                        freq = 57;
                        volt_1 = 480;
                        volt_2 = 5;
                        break;
                    case 6:
                        freq = 63;
                        volt_2 = 25;
                        break;
                    case 7:
                        volt_1 = 470;
                        break;
                }

                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt_1, freq);
                target.AcPower.SetOutput("AC2", volt_2, freq * 3);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                Thread.Sleep(5000);
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C39_SweepFreq_Asc_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication at certain mains frequencies.
 
        Implement :
        Set all relay connection switch to L-L setting.
        For(fFreq=47Hz;fFreq<=63Hz;fFreq+0.1Hz)
        {
          Set AC1Volt=480v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 20s;
 
          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 20s)
          Check received 4 bit command equal to sending command
        }  

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C39_SweepFreq_Asc_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 20 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            for (double freq = 470; freq <= 630; freq++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 480, freq / 10);
                target.AcPower.SetOutput("AC2", 0, freq / 10);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Flex.Log("Received CMD is " + cmdRcv);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C40_SweepFreq_Des_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication at certain mains frequencies.
 
        Implement :
        Set all relay connection switch to L-L setting.
        For(fFreq=63Hz;fFreq>=47Hz;fFreq-0.1Hz)
        {
          Set AC1Volt=380v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 20s;
 
          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 20s)
          Check received 4 bit command equal to sending command
        }

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C40_SweepFreq_Des_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 20 * 1000;

            //switch to L-L
            target.Relay.Close("Dry5");

            for (double freq = 630; freq <= 470; freq--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 380, freq / 10);
                target.AcPower.SetOutput("AC2", 0, freq / 10);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C41_FreqJump_Asc_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication when mains frequencies changes.
 
        Implement :
        Set all relay connection switch to L-L setting.
        For(fFreq=47Hz;fFreq<=63Hz;fFreq+3Hz)
        {
          Set AC1Volt=528v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 20s;
 
          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 20s)
          Check received 4 bit command equal to sending command
        }

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C41_FreqJump_Asc_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 20 * 1000;

            target.Relay.Close("Dry5");

            for (int freq = 47; freq <= 63; freq++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 528, freq);
                target.AcPower.SetOutput("AC2", 0, freq);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                freq = freq + 2;
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C42_FreqJump_Des_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication when mains frequencies changes.
 
        Implement :
        Set all relay connection switch to L-L setting.
        For(fFreq=63Hz;fFreq>=47Hz;fFreq-3Hz)
        {
          Set AC1Volt=342v, AC1Freq=fFreq to AC1 power supply; 
          Set AC2Volt=0v, AC2Freq=fFreq to AC2 power supply;  
          Wait 20s;
 
          If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
          Wait feedback of Rx wrapper while Rx received this command(time out 20s)
          Check received 4 bit command equal to sending command
        }

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C42_FreqJump_Des_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            target.Relay.Close("Dry5");

            for (int freq = 63; freq <= 47; freq--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", 342, freq);
                target.AcPower.SetOutput("AC2", 0, freq);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                freq = freq - 2;
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C43_SweepVolt_Asc_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication while mains frequency or voltage changed
 
        Implement :
 
        Set all relay connection switch to L-L setting.
        
        RXAPP_Start;
        For(Volt=342v; volt<=528v; volt++)
        {
            Set AC1Volt=volt, AC1Freq=60Hz to AC1 power supply; 
            Set AC2Volt=0v, AC2Freq=60Hz  to AC2 power supply;  
            Wait 30s;
 
             If latest CY1CMD!=8,
             {
                 Send CY1CMD8 from Tx to Rx ;
                 CY1CMD=15;
              }
              Else
              {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=8;
              }
              Wait feedback of Rx wrapper while Rx received this command(time out 10s)
              Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C43_SweepVolt_Asc_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //Set to L-L
            target.Relay.Close("Dry5");

            for (int volt = 342; volt <= 528; volt++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 60);
                target.AcPower.SetOutput("AC2", 0, 60);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C44_SweepVolt_Des_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication at certain mains frequencies.
 
        Implement :
 
        Set all relay connection switch to L-L setting.
        
        RXAPP_Start;
        For(volt=528v ;fvolt <= 342v;  volt--)
        {
             Set AC1Volt=volt, AC1Freq=50Hz to AC1 power supply; 
             Set AC2Volt=0v, AC2Freq=50Hz to AC2 power supply;  
             Wait 30s;
 
             If latest CY1CMD!=8,
             {
                 Send CY1CMD8 from Tx to Rx ;
                 CY1CMD=15;
              }
             Else
             {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=8;
              }
              Wait feedback of Rx wrapper while Rx received this command(time out 10s)
              Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C44_SweepVolt_Des_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //set to L-L
            target.Relay.Close("Dry5");

            for (int volt = 528; volt <= 324; volt--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 50);
                target.AcPower.SetOutput("AC2", 0, 50);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C45_VoltJump_Asc_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication when mains frequencies changes.
 
        Implement :
        Set all relay connection switch to L-L setting.
        
        RXAPP_Start;
        For(volt = 342v ; volt <= 528v; volt +5v)
        {
            Set AC1Volt=volt, AC1Freq=60Hz  to AC1 power supply; 
            Set AC2Volt=0v, AC2Freq=60Hz to AC2 power supply;  
            Wait 30s;
 
            If latest CY1CMD!=8,
            {
                 Send CY1CMD8 from Tx to Rx ;
                 CY1CMD=15;
            }
            Else
            {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=8;
            }
            Wait feedback of Rx wrapper while Rx received this command(time out 10s)
            Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C45_VoltJump_Asc_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //set to L-L
            target.Relay.Close("Dry5");

            for (int volt = 324; volt <= 528; volt++)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 60);
                target.AcPower.SetOutput("AC2", 0, 60);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                volt = volt + 4;
                target.Rxapp.Stop();
            }
        }
        #endregion

        #region Cmrg1_1_0_C46_VoltJump_Des_L2L
        [Test, Description(@"
        Narrative: Several software parameters may affect the communication when mains frequencies changes.
 
        Implement :
        Set all relay connection switch to L-L setting.
        
        RXAPP_Start;
        For(volt = 528v ; volt <=342v ; volt-5v)
        {
            Set AC1Volt=volt, AC1Freq=50Hz to AC1 power supply; 
            Set AC2Volt=0v, AC2Freq=50Hz to AC2 power supply;  
            Wait 30s;
 
            If latest CY1CMD!=8,
            {
                Send CY1CMD8 from Tx to Rx ;
                CY1CMD=15;
             }
             Else
             {
                  Send CY1CMD15 from Tx to Rx;
                  CY1CMD=8;
             }
             Wait feedback of Rx wrapper while Rx received this command(time out 10s)
             Check received 4 bit command equal to sending command
        }  
        RXAPP_Stop
        ")]
        [Category("Receiver(AUTO_L2L)")]
        public void Cmrg1_1_0_C46_VoltJump_Des_L2L()
        {
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            ushort cmd = 0;
            int sleepSec = 30 * 1000;

            //set to L-L
            target.Relay.Close("Dry5");

            for (int volt = 528; volt <= 342; volt--)
            {
                // Change AC1 mains to the given frequency and voltage
                target.AcPower.SetOutput("AC1", volt, 50);
                target.AcPower.SetOutput("AC2", 0, 50);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();

                history.Clear();
                switch (cmd)
                {
                    case 0:
                        cmd = 8;
                        break;
                    case 8:
                        cmd = 15;
                        break;
                    case 15:
                        cmd = 8;
                        break;
                }
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 10000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                volt = volt - 5;
                target.Rxapp.Stop();
            }
        }
        #endregion

        //---------------------------------------------------------

        #region Cmrg1_3_0_C1_DataDecoding_L1N
        [Test, Description(@"
        Narrative: Rx can support decode the Coded Mains CY1 protocol
 
         Implement :
        Make sure TX’s input is L1, N. RX’s input is L1, N.
        Set all relay connection switch to L-N setting.
 
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send CY1 CMD0 ~ CMD15 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received command equal to sending command;
        Sleep 15s for each switch over

        RXAPP_Stop
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_3_0_C1_DataDecoding_L1N()
        {
            UInt16 cmdRcv = 0;
            int sleepSec = 30 * 1000;

            //Messagebox: Please connect TX’s input with L1 and N, and connect RX’s input with L1 and N.

            //set to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            // test for dali
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("Send DALI Command is  " + cmd);
                target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                Flex.Log("Received DALI Command is  " + cmdRcv);
                Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_3_0_C2_DataDecoding_L2N
        [Test, Description(@"
        Narrative: Rx can support decode the Coded Mains CY1 protocol
 
        Implement :
        Make sure TX’s input is L1, N. RX’s input is L2, N.
        Set all relay connection switch to L-N setting.
 
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send CY1 CMD0 ~ CMD15 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received command equal to sending command;
        Sleep 15s for each switch over

        RXAPP_Stop
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_3_0_C2_DataDecoding_L2N()
        {
            UInt16 cmdRcv = 0;
            int sleepSec = 30 * 1000;

            //Messagebox: Please connect TX’s input with L1 and N, and connect RX’s input with L2 and N.

            //set to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            // test for dali
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("Send DALI Command is  " + cmd);
                target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                Flex.Log("Received DALI Command is  " + cmdRcv);
                Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_3_0_C3_DataDecoding_L3N
        [Test, Description(@"
        Narrative: Rx can support decode the Coded Mains CY1 protocol
 
        Implement :
        Make sure TX’s input is L1, N. RX’s input is L3, N.
        Set all relay connection switch to L-N setting.
 
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send CY1 CMD0 ~ CMD15 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received command equal to sending command;
        Sleep 15s for each switch over

        RXAPP_Stop
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_3_0_C3_DataDecoding_L3N()
        {
            UInt16 cmdRcv = 0;
            int sleepSec = 30 * 1000;

            //Messagebox: Please connect TX’s input with L1 and N, and connect RX’s input with L3 and N.

            //set to L-N
            target.Relay.Open("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            // test for dali
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("Send DALI Command is  " + cmd);
                target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                Flex.Log("Received DALI Command is  " + cmdRcv);
                Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_3_0_C4_DataDecoding_L1L2
        [Test, Description(@"
        Narrative: Rx can support decode the Coded Mains CY1 protocol
 
        Implement :
        Make sure TX’s input is L1, N. RX’s input is L1, L2.
        Set all relay connection switch to L-N setting.
 
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send CY1 CMD0 ~ CMD15 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received command equal to sending command;
        Sleep 15s for each switch over

        RXAPP_Stop
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_3_0_C4_DataDecoding_L1L2()
        {
            UInt16 cmdRcv = 0;
            int sleepSec = 30 * 1000;

            //Messagebox: Please connect TX’s input with L1 and N, and connect RX’s input with L1 and L2.

            //set to L-N
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            // test for dali
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("Send DALI Command is  " + cmd);
                target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                Flex.Log("Received DALI Command is  " + cmdRcv);
                Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_3_0_C5_DataDecoding_L1L3
        [Test, Description(@"
        Narrative: Rx can support decode the Coded Mains CY1 protocol
 
        Implement :
        Make sure TX’s input is L1, N. RX’s input is L1, L3.
        Set all relay connection switch to L-N setting.
 
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send CY1 CMD0 ~ CMD15 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received command equal to sending command;
        Sleep 15s for each switch over

        RXAPP_Stop
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_3_0_C5_DataDecoding_L1L3()
        {
            UInt16 cmdRcv = 0;
            int sleepSec = 30 * 1000;

            //Messagebox: Please connect TX’s input with L1 and N, and connect RX’s input with L1 and L3.

            //set to L-N
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            // test for dali
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("Send DALI Command is  " + cmd);
                target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                Flex.Log("Received DALI Command is  " + cmdRcv);
                Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_3_0_C6_DataDecoding_L2L3
        [Test, Description(@"
        Narrative: Rx can support decode the Coded Mains CY1 protocol
 
        Implement :
        Make sure TX’s input is L1, N. RX’s input is L2, L3.
        Set all relay connection switch to L-N setting.
 
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s
 
        RXAPP_Start
        Send CY1 CMD0 ~ CMD15 from Tx to Rx.
        Wait feedback of Rx wrapper while Rx received this command(time out 10s)
        Check received command equal to sending command;
        Sleep 15s for each switch over

        RXAPP_Stop
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_3_0_C6_DataDecoding_L2L3()
        {
            UInt16 cmdRcv = 0;
            int sleepSec = 30 * 1000;

            //Messagebox: Please connect TX’s input with L1 and N, and connect RX’s input with L2 and L3.

            //set to L-N
            target.Relay.Close("Dry5");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            // test for dali
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("Send DALI Command is  " + cmd);
                target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                Flex.Log("Received DALI Command is  " + cmdRcv);
                Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10 * 1000);
            }
            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_4_2_C1_Sensitivity_ART_L2N_HighVolt
        [Test, Description(@"
        This test check the summay latency between TX to RX

        Test steps:
        Set all relay connection switch to L-N setting.
 
        Set AC1Volt=305v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 30s

        RXAPP_Start
 
        for (Counter=0;Counter<2000;Counter++)
        {
           If latest CY1CMD!=8,
           {
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           {
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
            Record T1;
 
            Wait feedback of Rx wrapper while Rx received this command(time out 20s);
            Record T2;
 
            Check if received command equal to sending command;
 
            RT=T2-T1;
            If RT<=2s, ART2s++;
            If RT<=5s, ART5s++
            If RT<=10s, ART10s++;
            Sleep 20s;
        }
 
        Check if ART2s<2000*90%, fail the case.
        Check if ART5s<2000*99%, fail the case.
        Check if ART10s<2000*99.9%, fail the case.
 
        RXAPP_Stop
        Check if ART10s<2000*99.9%, fail the case.
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_4_2_C1_Sensitivity_ART_L2N_HighVolt()
        {
            int repeatCnt = 2000;
            int cnt_2s = 0;
            int cnt_5s = 0;
            int cnt_10s = 0;

            UInt16 cmdReq = 8;
            UInt16 cmdRev = 255;
            Stopwatch stopWatch = new Stopwatch();
            long deltaTime_ms = 0;

            target.AcPower.SetOutput("AC1", 305, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(30 * 1000);
            target.Rxapp.Start();

            for (int repeat = 0; repeat < repeatCnt; repeat++)
            {
                history.Clear();
                //request to TX cmd
                if (Cmd2Dry(cmdReq) != 255)
                {
                    DrySwitch(Cmd2Dry(cmdReq));
                }
                stopWatch.Restart();
                //check RX received value
                target.Rxapp.Get_Cb_NewCmd(out cmdRev, ref history, 20 * 1000);
                //Thread.Sleep(10 * 1000);
                //target.Rxapp.Get_LatestCy1Cmd(out cmdRev, ref history, 2000);

                stopWatch.Stop();
                Assert.AreEqual(cmdReq, cmdRev, "CY1 CMD Expected: " + cmdReq + " ,but Received " + cmdRev);

                //check & count the latency
                deltaTime_ms = stopWatch.ElapsedMilliseconds;
                if (deltaTime_ms < 2000)
                {
                    cnt_2s++;
                }
                if (deltaTime_ms < 5000)
                {
                    cnt_5s++;
                }
                if (deltaTime_ms < 10000)
                {
                    cnt_10s++;
                }
                //toggle cmdReq
                cmdReq = (cmdReq == 8) ? ((ushort)15) : ((ushort)8);
            }
            target.Rxapp.Stop();

            Assert.GreaterOrEqual(cnt_2s, repeatCnt * 0.9, "Failed for the 90% resp time within 2 sec");
            Assert.GreaterOrEqual(cnt_5s, repeatCnt * 0.99, "Failed for the 99% resp time within 5 sec");
            Assert.GreaterOrEqual(cnt_10s, repeatCnt * 0.999, "Failed for the 90% resp time within 10 sec");

            Flex.Log("<=2s " + cnt_2s + " times, <=5s " + cnt_5s + " <=times, 10s " + cnt_10s);
            Flex.Log("Total Repeat toggle CMD8/CMD15 for " + repeatCnt + "times");
        }
        #endregion

        /*Li Jay*/

        #region Cmrg1_4_2_C1_Sensitivity_ART_L2N_HighVolt_110
        [Test, Description(@"
        
        This test is to verify the Coded Mains receiver shall respond to a Coded Mains command within 2 seconds in 90%, within 5 seconds in 99% and within 10 seconds in 99.9%.
        Implement :
        Set all relay connection switch to L-N setting.
        Set AC1Volt=305v, AC1Freq=63Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=63Hz*3, to AC2 power supply; 
        Sleep 30s
        RXAPP_Start

        for (Counter=0;Counter<2000;Counter++)
        {      
               If latest CY1CMD!=8;
           {   
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
           }
           Else
           {    
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
            Record T1;
            Wait feedback of Rx wrapper while Rx received this command(time out 20s);
            Record T2;
            Check if received command equal to sending command;
            RT=T2-T1;
            If RT<=2s, ART2s++;
            If RT<=5s, ART5s++
            If RT<=10s, ART10s++;
            Sleep 20s;
        }
        Check if ART2s<2000*90%, fail the case.
        Check if ART5s<2000*99%, fail the case.
        Check if ART10s<2000*99.9%, fail the case.
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO)")]
        public void Cmrg1_4_2_C1_Sensitivity_ART_L2N_HighVolt_110()
        {
            int repeatCnt = 2000;
            int cnt_2s = 0;
            int cnt_5s = 0;
            int cnt_10s = 0;

            UInt16 cmdReq = 8;
            UInt16 cmdRev = 255;
            Stopwatch stopWatch = new Stopwatch();
            long deltaTime_ms = 0;
            double volt = 0;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            target.AcPower.SetOutput("AC1", 305, 63);
            target.AcPower.SetOutput("AC2", 5, 63 * 3);
            Thread.Sleep(30 * 1000);

            target.Rxapp.Start();

            for (int repeat = 0; repeat < repeatCnt; repeat++)
            {
                history.Clear();
                //request to TX cmd
                if (Cmd2Dry(cmdReq) != 255)
                {
                    DrySwitch(Cmd2Dry(cmdReq));
                }
                stopWatch.Restart();
                //check RX received value
                target.Rxapp.Get_Cb_NewCmd(out cmdRev, ref history, 20 * 1000);
                Assert.AreEqual(cmdReq, cmdRev, "CY1 CMD Expected: " + cmdReq + " ,but Received " + cmdRev);
                Flex.Log("Received command " + cmdRev);
                Thread.Sleep(600);
                target.MultiMeter.GetActualVoltage("MM1", ref volt);
                stopWatch.Stop();
                if (cmdReq == 8)
                {
                    Assert.LessOrEqual(volt, 4.1 * 1.1);
                    Assert.GreaterOrEqual(volt, 4.1 * 0.9);
                }
                else
                {
                    Assert.LessOrEqual(volt, 8 * 1.1);
                    Assert.GreaterOrEqual(volt, 8 * 0.9);
                }

                //check & count the latency
                deltaTime_ms = stopWatch.ElapsedMilliseconds;
                if (deltaTime_ms < 2000)
                {
                    cnt_2s++;
                }
                if (deltaTime_ms < 5000)
                {
                    cnt_5s++;
                }
                if (deltaTime_ms < 10000)
                {
                    cnt_10s++;
                }
                //toggle cmdReq
                cmdReq = (cmdReq == 8) ? ((ushort)15) : ((ushort)8);
            }
            target.Rxapp.Stop();
            Flex.Log("cnt_2s " + cnt_2s);
            Flex.Log("cnt_5s " + cnt_5s);
            Flex.Log("cnt_10s " + cnt_10s);

            Assert.GreaterOrEqual(cnt_2s, repeatCnt * 0.9, "Failed for the 90% resp time within 2 sec");
            Assert.GreaterOrEqual(cnt_5s, repeatCnt * 0.99, "Failed for the 99% resp time within 5 sec");
            Assert.GreaterOrEqual(cnt_10s, repeatCnt * 0.999, "Failed for the 99.9% resp time within 10 sec");

            Flex.Log("<=2s " + cnt_2s + " times, <=5s " + cnt_5s + " <=times, 10s " + cnt_10s);
            Flex.Log("Total Repeat toggle CMD8/CMD15 for " + repeatCnt + "times");
        }
        #endregion

        #region Cmrg1_4_2_C3_Sensitivity_MotherFather_110
        [Test, Description(@"
        This test is to verify the receiver shall correctly decode the command while continuously power cycle the receiver.
        Implement :
        Set all relay connection switch to L-N setting.
        for (Counter=0;Counter<500;Counter++)
        {
            Set AC1Volt=277v, AC1Freq=60Hz, to AC1 power supply; 
            Set AC2Volt=5v, AC2Freq=60Hz*3, to AC2 power supply; 

            Sleep 10s

            RXAPP_Start  

            Send CY1CMD8 from Tx to Rx ;

            Wait feedback of Rx wrapper while Rx received this command(time out 20s);

            Check if received command equal to sending command;
             
            RXAPP_Stop

            Set AC1 output to OFF
            Set AC2 output to OFF

}
        ")]
        [Category("Receiver(AUTO)")]
        public void Cmrg1_4_2_C3_Sensitivity_MotherFather_110()
        {
            int repeatCnt = 50;
            UInt16 cmdRcv = 0;
            UInt16 dry = 0;
            int sleepSec = 10 * 1000;
            ushort cmd = 8;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            for (int repeat = 0; repeat < repeatCnt; repeat++)
            {
                // Change AC1 mains to the given frequency
                target.AcPower.SetOutput("AC1", 277, 60);
                target.AcPower.SetOutput("AC2", 5, 60 * 3);
                Thread.Sleep(sleepSec);

                target.Rxapp.Start();
                history.Clear();
                // Set Relay board based on CY1 command
                dry = Cmd2Dry(cmd);
                if (dry != 255)
                {
                    DrySwitch(dry);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20 * 1000);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                }
                target.Rxapp.Stop();
                target.AcPower.OutputOff("AC1");
                target.AcPower.OutputOff("AC2");
            }
        }
        #endregion
        #region Cmrg1_4_2_C4_Sensitivity_ART_L2N_230v50Hz_110
        [Test, Description(@"
        
        This test is to verify the Coded Mains receiver shall respond to a Coded Mains command within 2 seconds in 90%, within 5 seconds in 99% and within 10 seconds in 99.9%.
        Implement :

        Set all relay connection switch to L-N setting.
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 30s

        RXAPP_Start

        for (Counter=0;Counter<2000;Counter++)
        {      
               If latest CY1CMD!=8,
           {   
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           { 
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
            Record T1;
            Wait feedback of Rx wrapper while Rx received this command(time out 20s);
            Record T2;
            Check if received command equal to sending command;
            RT=T2-T1;
            If RT<=2s, ART2s++;
            If RT<=5s, ART5s++
            If RT<=10s, ART10s++;
            Sleep 20s;
        }
        Check if ART2s<2000*90%, fail the case.
        Check if ART5s<2000*99%, fail the case.
        Check if ART10s<2000*99.9%, fail the case.
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO)")]
        public void Cmrg1_4_2_C4_Sensitivity_ART_L2N_230v50Hz_110()
        {
            int repeatCnt = 20;
            int cnt_2s = 0;
            int cnt_5s = 0;
            int cnt_10s = 0;

            UInt16 cmdReq = 8;
            UInt16 cmdRev = 255;
            Stopwatch stopWatch = new Stopwatch();
            long deltaTime_ms = 0;
            double volt = 0;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(30 * 1000);

            target.Rxapp.Start();

            for (int repeat = 0; repeat < repeatCnt; repeat++)
            {
                history.Clear();
                //request to TX cmd
                if (Cmd2Dry(cmdReq) != 255)
                {
                    DrySwitch(Cmd2Dry(cmdReq));
                }
                stopWatch.Restart();
                //check RX received value
                target.Rxapp.Get_Cb_NewCmd(out cmdRev, ref history, 20 * 1000);
                Assert.AreEqual(cmdReq, cmdRev, "CY1 CMD Expected: " + cmdReq + " ,but Received " + cmdRev);
                Flex.Log("Received command " + cmdRev);
                Thread.Sleep(600);
                target.MultiMeter.GetActualVoltage("MM1", ref volt);
                stopWatch.Stop();
                if (cmdReq == 8)
                {
                    Assert.LessOrEqual(volt, 4.1 * 1.1 / 1000);
                    Assert.GreaterOrEqual(volt, 4.1 * 0.9 / 1000);
                }
                else
                {
                    Assert.LessOrEqual(volt, 8 * 1.1 / 1000);
                    Assert.GreaterOrEqual(volt, 8 * 0.9 / 1000);
                }
                //check & count the latency
                deltaTime_ms = stopWatch.ElapsedMilliseconds;
                if (deltaTime_ms < 2000)
                {
                    cnt_2s++;
                }
                if (deltaTime_ms < 5000)
                {
                    cnt_5s++;
                }
                if (deltaTime_ms < 10000)
                {
                    cnt_10s++;
                }
                //toggle cmdReq
                cmdReq = (cmdReq == 8) ? ((ushort)15) : ((ushort)8);
            }
            target.Rxapp.Stop();

            Flex.Log("cnt_2s " + cnt_2s);
            Flex.Log("cnt_5s " + cnt_5s);
            Flex.Log("cnt_10s " + cnt_10s);

            Assert.GreaterOrEqual(cnt_2s, repeatCnt * 0.9, "Failed for the 90% resp time within 2 sec");
            Assert.GreaterOrEqual(cnt_5s, repeatCnt * 0.99, "Failed for the 99% resp time within 5 sec");
            Assert.GreaterOrEqual(cnt_10s, repeatCnt * 0.999, "Failed for the 99.9% resp time within 10 sec");

            Flex.Log("<=2s " + cnt_2s + " times, <=5s " + cnt_5s + " <=times, 10s " + cnt_10s);
            Flex.Log("Total Repeat toggle CMD8/CMD15 for " + repeatCnt + "times");
        }
        #endregion
        #region Cmrg1_4_2_C5_Sensitivity_ART_L2N_120v60Hz_110
        [Test, Description(@"
        
        This test is to verify the Coded Mains receiver shall respond to a Coded Mains command within 2 seconds in 90%, within 5 seconds in 99% and within 10 seconds in 99.9%.
        Implement :
        Set all relay connection switch to L-N setting.
  
        Set AC1Volt=120v, AC1Freq=60Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=60Hz*3, to AC2 power supply; 
        Sleep 30s

        RXAPP_Start

        for (Counter=0;Counter<2000;Counter++)
        {   If latest CY1CMD!=8,
           {   Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           { Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
            Record T1;
            Wait feedback of Rx wrapper while Rx received this command(time out 20s);
            Record T2;
            Check if received command equal to sending command;
            RT=T2-T1;
            If RT<=2s, ART2s++;
            If RT<=5s, ART5s++
            If RT<=10s, ART10s++;
            Sleep 20s;
        }
        Check if ART2s<2000*90%, fail the case.
        Check if ART5s<2000*99%, fail the case.
        Check if ART10s<2000*99.9%, fail the case.
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO)")]
        public void Cmrg1_4_2_C5_Sensitivity_ART_L2N_120v60Hz_110()
        {
            int repeatCnt = 20;
            int cnt_2s = 0;
            int cnt_5s = 0;
            int cnt_10s = 0;

            UInt16 cmdReq = 8;
            UInt16 cmdRev = 255;
            Stopwatch stopWatch = new Stopwatch();
            long deltaTime_ms = 0;
            double volt = 0;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            target.AcPower.SetOutput("AC1", 120, 60);
            target.AcPower.SetOutput("AC2", 5, 60 * 3);
            Thread.Sleep(30 * 1000);
            target.Rxapp.Start();

            for (int repeat = 0; repeat < repeatCnt; repeat++)
            {
                history.Clear();
                //request to TX cmd
                if (Cmd2Dry(cmdReq) != 255)
                {
                    DrySwitch(Cmd2Dry(cmdReq));
                }
                stopWatch.Restart();
                //check RX received value
                target.Rxapp.Get_Cb_NewCmd(out cmdRev, ref history, 20 * 1000);
                Assert.AreEqual(cmdReq, cmdRev, "CY1 CMD Expected: " + cmdReq + " ,but Received " + cmdRev);
                //Thread.Sleep(1000);
                target.MultiMeter.GetActualVoltage("MM1", ref volt);
                stopWatch.Stop();
                if (cmdReq == 8)
                {
                    Assert.LessOrEqual(volt, 4.1 * 1.1 / 1000);
                    Assert.GreaterOrEqual(volt, 4.1 * 0.9 / 1000);
                }
                else
                {
                    Assert.LessOrEqual(volt, 8 * 1.1 / 1000);
                    Assert.GreaterOrEqual(volt, 8 * 0.9 / 1000);
                }
                //check & count the latency
                deltaTime_ms = stopWatch.ElapsedMilliseconds;
                if (deltaTime_ms < 2000)
                {
                    cnt_2s++;
                }
                if (deltaTime_ms < 5000)
                {
                    cnt_5s++;
                }
                if (deltaTime_ms < 10000)
                {
                    cnt_10s++;
                }
                //toggle cmdReq
                cmdReq = (cmdReq == 8) ? ((ushort)15) : ((ushort)8);
            }
            target.Rxapp.Stop();

            Assert.GreaterOrEqual(cnt_2s, repeatCnt * 0.9, "Failed for the 90% resp time within 2 sec");
            Assert.GreaterOrEqual(cnt_5s, repeatCnt * 0.99, "Failed for the 99% resp time within 5 sec");
            Assert.GreaterOrEqual(cnt_10s, repeatCnt * 0.999, "Failed for the 99.9% resp time within 10 sec");

            Flex.Log("<=2s " + cnt_2s + " times, <=5s " + cnt_5s + " <=times, 10s " + cnt_10s);
            Flex.Log("Total Repeat toggle CMD8/CMD15 for " + repeatCnt + "times");
        }
        #endregion
        #region Cmrg1_4_2_C6_Sensitivity_ART_L2N_277v60Hz_110
        [Test, Description(@"
        
        This test is to verify the Coded Mains receiver shall respond to a Coded Mains command within 2 seconds in 90%, within 5 seconds in 99% and within 10 seconds in 99.9%.
        Implement :
        Set all relay connection switch to L-N setting.
        Set AC1Volt=277v, AC1Freq=60Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=60Hz*3, to AC2 power supply; 
        Sleep 30s
        RXAPP_Start

        for (Counter=0;Counter<2000;Counter++)
        {   
               If latest CY1CMD!=8,
           {   
               Send CY1CMD8 from Tx to Rx ;
               CY1CMD=15;
            }
           Else
           {   
                Send CY1CMD15 from Tx to Rx;
                CY1CMD=8;
            }
            Record T1;
            Wait feedback of Rx wrapper while Rx received this command(time out 20s);
            Record T2;
            Check if received command equal to sending command;
            RT=T2-T1;
            If RT<=2s, ART2s++;
            If RT<=5s, ART5s++
            If RT<=10s, ART10s++;
            Sleep 20s;
        }
        Check if ART2s<2000*90%, fail the case.
        Check if ART5s<2000*99%, fail the case.
        Check if ART10s<2000*99.9%, fail the case.
        RXAPP_Stop

        ")]
        [Category("Receiver(AUTO)")]
        public void Cmrg1_4_2_C6_Sensitivity_ART_L2N_277v60Hz_110()
        {
            int repeatCnt = 10;
            int cnt_2s = 0;
            int cnt_5s = 0;
            int cnt_10s = 0;

            UInt16 cmdReq = 8;
            UInt16 cmdRev = 255;
            Stopwatch stopWatch = new Stopwatch();
            long deltaTime_ms = 0;
            double volt = 0;

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            target.AcPower.SetOutput("AC1", 277, 60);
            target.AcPower.SetOutput("AC2", 5, 60 * 3);
            Thread.Sleep(30 * 1000);
            target.Rxapp.Start();

            for (int repeat = 0; repeat < repeatCnt; repeat++)
            {
                history.Clear();
                //request to TX cmd
                if (Cmd2Dry(cmdReq) != 255)
                {
                    DrySwitch(Cmd2Dry(cmdReq));
                }
                stopWatch.Restart();
                //check RX received value
                target.Rxapp.Get_Cb_NewCmd(out cmdRev, ref history, 20 * 1000);
                Assert.AreEqual(cmdReq, cmdRev, "CY1 CMD Expected: " + cmdReq + " ,but Received " + cmdRev);
                //Thread.Sleep(100);
                target.MultiMeter.GetActualVoltage("MM1", ref volt);
                stopWatch.Stop();
                if (cmdReq == 8)
                {
                    Assert.LessOrEqual(volt, 4.1 * 1.1 / 1000);
                    Assert.GreaterOrEqual(volt, 4.1 * 0.9 / 1000);
                }
                else
                {
                    Assert.LessOrEqual(volt, 8 * 1.1 / 1000);
                    Assert.GreaterOrEqual(volt, 8 * 0.9 / 1000);
                }
                //check & count the latency
                deltaTime_ms = stopWatch.ElapsedMilliseconds;
                if (deltaTime_ms < 2000)
                {
                    cnt_2s++;
                }
                if (deltaTime_ms < 5000)
                {
                    cnt_5s++;
                }
                if (deltaTime_ms < 10000)
                {
                    cnt_10s++;
                }
                //toggle cmdReq
                cmdReq = (cmdReq == 8) ? ((ushort)15) : ((ushort)8);
            }
            target.Rxapp.Stop();

            Assert.GreaterOrEqual(cnt_2s, repeatCnt * 0.9, "Failed for the 90% resp time within 2 sec");
            Assert.GreaterOrEqual(cnt_5s, repeatCnt * 0.99, "Failed for the 99% resp time within 5 sec");
            Assert.GreaterOrEqual(cnt_10s, repeatCnt * 0.999, "Failed for the 90% resp time within 10 sec");

            Flex.Log("<=2s " + cnt_2s + " times, <=5s " + cnt_5s + " <=times, 10s " + cnt_10s);
            Flex.Log("Total Repeat toggle CMD8/CMD15 for " + repeatCnt + "times");
        }
        #endregion


        /*End*/

        #region Cmrg1_5_0_C1_SceneMapping
        [Test, Description(@"
        Narrative: Rx can support correctly mapping Scene to dimming levels.
            4 bit Command    Percentage 
            0   
            1   
            2   
            3 100% 
            4 OFF 
            5 1% 
            6 10% 
            7 35% 
            8 50% 
            9 60% 
            10 75% 
            11 80% 
            12 85% 
            13 90% 
            14 95% 
            15 100% 

        Implement :
            Set AC1Volt=277v, AC1Freq=60Hz, to AC1 power supply; 
            Set AC2Volt=5v, AC2Freq=60Hz*3, to AC2 power supply; 
            Sleep 30s
            Send CY1 CMD3 ~ CMD15 from Tx to Rx.
            Get Percentage
            Checking Mapping Percentage equals to expected percentage;
            Sleep 10s for each switch over
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_5_0_C1_SceneMapping()
        {
            ushort percentageRcv;
            ushort RcdCMD;
            UInt16 sleepSec = 30 * 1000;
            UInt16[] thdTable = { 10000, 0, 100, 1000, 3500, 5000, 6000, 7500, 8000, 8500, 9000, 9500, 10000 };

            target.AcPower.SetOutput("AC1", 277, 60);
            target.AcPower.SetOutput("AC2", 5, 60 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (short cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("CMD is " + cmd);

                target.Rxapp.Get_Cb_NewCmd(out RcdCMD, ref history, timeout: 10000);
                Flex.Log("RcdCMD is " + RcdCMD);

                target.Rxapp.Get_DimPercentage(out percentageRcv, ref history, timeout: 10000);
                Flex.Log("Percentage is " + percentageRcv);

                Assert.AreEqual(thdTable[cmd - 3], percentageRcv, "Failed due to" + cmd);
                Thread.Sleep(10 * 1000);
            }
            target.Rxapp.Stop();

        }
        #endregion

        #region Cmrg1_6_2_C1_PowerUp110v
        [Test, Description(@"
        Narrative: The power-up dimlevel of the receiver for 1-10v shall be 100%
        Also can oberserve the lamp behavior directly, no flash is expected
 
        Preconcition:
        Receiver connected with a 1-10v driver and lamp.
 
        Implement :
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        if (t=0s, t<10s, t++)
        {
         check dimming level of 1-10v output= 100%;
        }
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_6_2_C1_PowerUp110v()
        {
            short volt = 0;
            double actvolt = 0;
            int sleepSec = 20 * 1000;

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Get_A1To10vOutput(out volt, ref history, timeout: 10000);
            Assert.AreEqual(1000, volt);

            for (int t = 0; t < 5; t++)
            {
                history.Clear();
                target.MultiMeter.GetActualVoltage("MM1", ref actvolt);
                Assert.GreaterOrEqual(actvolt, (volt * 0.9 / 1000));
                Assert.LessOrEqual(actvolt, (volt * 1.1 / 1000));
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Cmrg1_6_2_C2_110VOutputMapping
        [Test, Description(@"
        Narrative: Rx shall support 1-10v output, linear interpolation inbetween 1V = 10%, 8V = 100%.
        Tolerance is  +/-10%
 
        Precondition:
        Receiver connected with a 1-10v Ballast and lamp.
 
        Implement :
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 20s
 
        Send CY1 CMD3~15
        Check 1-10v output should be:
        CMD3=8v; 
        CMD4=1v;
        CMD5=1v;
        CMD6=1v;
        CMD7=2.9v;
        CMD8=4.1v;
        CMD9=4.9v;
        CMD10=6v;
        CMD11=6.4v;
        CMD12=6.8v;
        CMD13=7.2v; 
        CMD14=7.6v;
        CMD15=8v;
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_6_2_C2_110VOutputMapping()
        {
            short volt = 0;
            double actvolt = 0;
            int sleepSec = 20 * 1000;

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();

                switch (cmd)
                {
                    case 3:
                        IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 4:
                        objIdali = new Dali_GO_TO_SCENE_1();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 5:
                        objIdali = new Dali_GO_TO_SCENE_2();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 6:
                        objIdali = new Dali_GO_TO_SCENE_3();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 7:
                        objIdali = new Dali_GO_TO_SCENE_4();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 8:
                        objIdali = new Dali_GO_TO_SCENE_5();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 9:
                        objIdali = new Dali_GO_TO_SCENE_6();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 10:
                        objIdali = new Dali_GO_TO_SCENE_7();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 11:
                        objIdali = new Dali_GO_TO_SCENE_8();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 12:
                        objIdali = new Dali_GO_TO_SCENE_9();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 13:
                        objIdali = new Dali_GO_TO_SCENE_10();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 14:
                        objIdali = new Dali_GO_TO_SCENE_11();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                    case 15:
                        objIdali = new Dali_GO_TO_SCENE_12();
                        target.Dali.SendDaliCommand("Dali_tx", objIdali);
                        break;
                }
                Flex.Log("Send DALI Command is  " + cmd);
                Thread.Sleep(1000);

                // get 1-10v output of Rx
                target.Rxapp.Get_A1To10vOutput(out volt, ref history, timeout: 10000);
                switch (cmd)
                {
                    case 15:
                    case 3:
                        Assert.AreEqual(8000, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 14:
                        Assert.LessOrEqual(volt, 7600, "Fail for receiving CY1 CMD " + cmd);
                        Assert.GreaterOrEqual(volt, 7590, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 13:
                        Assert.LessOrEqual(volt, 7200, "Fail for receiving CY1 CMD " + cmd);
                        Assert.GreaterOrEqual(volt, 7190, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 12:
                        Assert.LessOrEqual(volt, 6800, "Fail for receiving CY1 CMD " + cmd);
                        Assert.GreaterOrEqual(volt, 6790, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 11:
                        Assert.LessOrEqual(volt, 6400, "Fail for receiving CY1 CMD " + cmd);
                        Assert.GreaterOrEqual(volt, 6390, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 10:
                        Assert.LessOrEqual(volt, 6050, "Fail for receiving CY1 CMD " + cmd);
                        Assert.GreaterOrEqual(volt, 6000, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 9:
                        Assert.AreEqual(4900, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 8:
                        Assert.AreEqual(4100, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 7:
                        Assert.LessOrEqual(volt, 2950, "Fail for receiving CY1 CMD " + cmd);
                        Assert.GreaterOrEqual(volt, 2900, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 6:
                    case 5:
                    case 4:
                        Assert.AreEqual(1000, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                }

                Thread.Sleep(sleepSec);

                // get 1-10v actual voltage from multimeter
                target.MultiMeter.GetActualVoltage("MM1", ref actvolt);
                Flex.Log("Actual voltage: " + actvolt);
                Assert.GreaterOrEqual(actvolt, (volt * 0.9 / 1000));
                Assert.LessOrEqual(actvolt, (volt * 1.1 / 1000));
            }

            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_6_2_C3_NoSignal_110v
        [Test, Description(@"
        Narrative: When no Coded Mains signal has been decoded in the past 5 minutes, the receiver will send the Fallback dimlevel

        Precondition:
        Receiver connected with a 1-10v Ballast and lamp.
 
        Implement :
        Switch relay to 1-10v output;
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 5*60s;
        Check the output of 1-10v, should send out 100%(8v);
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_6_2_C3_NoSignal_110v()
        {
            short volt = 0;
            double actvolt = 0;
            int sleepSec = 20 * 1000;
            ushort RevCMD;

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();
            if (Cmd2Dry(6) != 255)
            {
                DrySwitch(Cmd2Dry(6));
            }
            target.Rxapp.Get_Cb_NewCmd(out RevCMD, ref history, timeout: 10000);
            if (Cmd2Dry(0) != 255)
            {
                DrySwitch(Cmd2Dry(0));
            }
            Thread.Sleep(330 * 1000);
            target.Rxapp.Get_Cb_NewCmd(out RevCMD, ref history, timeout: 10000);
            Assert.AreEqual(3, RevCMD, "Fail for receiving Fallback CMD");

            history.Clear();
            target.Rxapp.Get_A1To10vOutput(out volt, ref history, timeout: 10000);
            Assert.AreEqual(8000, volt, "Fail for setting Fallback output");

            target.MultiMeter.GetActualVoltage("MM1", ref actvolt);
            Assert.GreaterOrEqual(actvolt, (volt * 0.9 / 1000));
            Assert.LessOrEqual(actvolt, (volt * 1.1 / 1000));

            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_6_2_C4_2Drivers_110v
        [Test, Description(@"
        Narrative: Narrative: Rx shall support two 1-10v drivers with one receiver.
        Tolerance is  +/-10%
 
        Precondition:
        Receiver connected with two 1-10v Ballasts and lamps.
 
        Implement :
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 20s
 
        Send CY1 CMD3~15
        Check 1-10v output should be:
        CMD3=8v; 
        CMD4=1v;
        CMD5=1v;
        CMD6=1v;
        CMD7=2.9v;
        CMD8=4.1v;
        CMD9=4.9v;
        CMD10=6v;
        CMD11=6.4v;
        CMD12=6.8v;
        CMD13=7.2v; 
        CMD14=7.6v;
        CMD15=8v;
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_6_2_C4_2Drivers_110v()
        {
            short volt = 0;
            double actualvolt1 = 0, actualvolt2 = 0;
            ushort cmdRcv;
            int sleepSec = 10 * 1000;

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();
            for (ushort cmd = 8; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                if (Cmd2Dry(cmd) != 255)
                {
                    DrySwitch(Cmd2Dry(cmd));
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                    Flex.Log("Received CY1 CMD is " + cmdRcv);
                }
                else
                {
                    continue;
                }
                // get 1-10v output of Rx
                target.Rxapp.Get_A1To10vOutput(out volt, ref history, timeout: 20000);
                switch (cmd)
                {
                    case 15:
                    case 3:
                        Assert.AreEqual(8000, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 14:
                        Assert.AreEqual(7600, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 13:
                        Assert.AreEqual(7200, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 12:
                        Assert.AreEqual(6800, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 11:
                        Assert.AreEqual(6400, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 10:
                        Assert.AreEqual(6000, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 9:
                        Assert.AreEqual(4900, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 8:
                        Assert.AreEqual(4100, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 7:
                        Assert.AreEqual(2900, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                    case 6:
                    case 5:
                    case 4:
                        Assert.AreEqual(1000, volt, "Fail for receiving CY1 CMD " + cmd);
                        break;
                }
                Thread.Sleep(sleepSec);

                // get 1-10v actual voltage from multimeter
                target.MultiMeter.GetActualVoltage("MM1", ref actualvolt1);
                target.MultiMeter.GetActualVoltage("MM2", ref actualvolt2);
                Assert.GreaterOrEqual(actualvolt1, (volt * 0.9 / 1000));
                Assert.LessOrEqual(actualvolt1, (volt * 1.1 / 1000));
                Assert.GreaterOrEqual(actualvolt2, (volt * 0.9 / 1000));
                Assert.LessOrEqual(actualvolt2, (volt * 1.1 / 1000));
            }

            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_6_1_C1_PowerUpDALI
        [Test, Description(@"
        Narrative: The power-up dimlevel of the receiver shall be 10%

        Preconcition:
        Receiver connected with a dali driver and lamp.
 
        Implement :
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        if (t=0s, t<10s, t++)
        {
         check dimming level of Dali output= 10% every 11 seconds;
        }
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_6_1_C1_PowerUpDALI()
        {
            byte rcvDaliData = 0;


            //Messagebox: Prepare to monitor the lamp behavior
            MessageBox.Show("Please prepare to monitor the lamp behavior!");

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);

            //Messagebox: did you observe lamp flicker of DALI Ballast
            Assert.False(UserInput.YesNoBox("Rx DALI Test", "Did you observe lamp flicker of DALI Ballast?"), "Fail to observe lamp flicker of DALI Ballast ");

            for (int i = 0; i < 5; i++)
            {
                target.Dali.StartSniffer("Dali_rx");
                target.Dali.StopSniffer("Dali_rx", 11000);
                history.Clear();
                target.Dali.GetCmd("Dali_rx", DaliCommands.DIRECT_ARC_POWER_CONTROL, 2, ref rcvDaliData);
                Flex.Log("received cmd:  " + rcvDaliData + "test times is" + (i + 1));
                Assert.AreEqual(169, rcvDaliData, "Fail to get the 10% dim level " + rcvDaliData);
            }
        }
        #endregion

        #region Cmrg1_6_1_C2_DALIOutputMapping
        [Test, Description(@"
        Narrative: Rx shall generate DALI broadcast DAPC(level) (direct arc power control) commands. 
        When a dimlevel of 0% is to be implemented a DALI OFF command will be broadcasted.

        4 bit Command	Percentage	DimLevel	ArcPower
        0	 	 	 
        1	 	 	 
        2	 	 	 
        3	100%	    10000	        254
        4	OFF	        0	            0
        5	1%	        1	            85
        6	10%	        5000	        169
        7	35%	        7689	        215
        8	50%	        8495	        229
        9	60% 	    8860	        235
        10	75%	        9654	        243
        11	80%	        9483	        245
        12	85%	        9612	        247
        13	90%	        9741	        250
        14	95%	        9870	        252
        15	100%	    10000	        254

        Precondition:
        Receiver connected with a Dali buster.
 
        Implement :
 
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 20s
 
        Send CY1 CMD3 
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 254;
        Sleep 15s
 
        Send CY1 CMD4
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast OFF;
        Sleep 15s
 
        Send CY1 CMD5 
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 85;
        Sleep 15s
 
        Send CY1 CMD6
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 169;
        Sleep 15s
 
        Send CY1 CMD7
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 215;
        Sleep 15s
 
        Send CY1 CMD8 
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 229;
        Sleep 15s
 
        Send CY1 CMD9
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 235;
        Sleep 15s
 
 
        Send CY1 CMD10 
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 243;
        Sleep 15s
 
        Send CY1 CMD11 
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 245;
        Sleep 15s
 
        Send CY1 CMD12
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 247;
        Sleep 15s
 
        Send CY1 CMD13
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 250;
        Sleep 15s
 
        Send CY1 CMD14 
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 252;
        Sleep 15s
 
        Send CY1 CMD15
        Sleep 10s
        Get Dali output command from dali buster
        Check if useded command is broadcast DAPC and setting value is 254;
        Sleep 15s
        ")]
        [Category("Receiver(AUTO_L2N)")]
        public void Cmrg1_6_1_C2_DALIOutputMapping()
        {

            UInt16 sleepSec = 20 * 1000;
            UInt16 _sleepSec = 15 * 1000;
            byte rcvDaliData = 0;

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);
            IDaliObject[] objIdali = new IDaliObject[] {new Dali_GO_TO_SCENE_0(),new Dali_GO_TO_SCENE_1(),new Dali_GO_TO_SCENE_2(),
                                                        new Dali_GO_TO_SCENE_3(),new Dali_GO_TO_SCENE_4(),new Dali_GO_TO_SCENE_5(),
                                                        new Dali_GO_TO_SCENE_6(),new Dali_GO_TO_SCENE_7(),new Dali_GO_TO_SCENE_8(),
                                                        new Dali_GO_TO_SCENE_9(),new Dali_GO_TO_SCENE_10(),new Dali_GO_TO_SCENE_11(),
                                                        new Dali_GO_TO_SCENE_12()};
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                target.Dali.SendDaliCommand("Dali_tx", objIdali[cmd - 3]);

                Flex.Log("Send DALI Command is  " + cmd);
                target.Dali.StartSniffer("Dali_rx");
                target.Dali.StopSniffer("Dali_rx", 20000);

                target.Dali.GetCmd("Dali_rx", (cmd != 4) ? (DaliCommands.DIRECT_ARC_POWER_CONTROL) : (DaliCommands.OFF), 2, ref rcvDaliData);
                Flex.Log("DAPC valuse is " + rcvDaliData);

                UInt16[] ExpectValue = { 254, 0, 85, 169, 215, 229, 235, 243, 245, 247, 250, 252, 254 };
                Assert.AreEqual(ExpectValue[cmd - 3], rcvDaliData, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(_sleepSec);
            }

        }
        #endregion

        #region Cmrg1_6_1_C3_NoSignal_DALI_MA
        [Test, Description(@"
        Narrative: When no Coded Mains signal has been decoded in the past 5 minutes, the receiver will send the Fallback dimlevel
 
        Preconcition:
        Receiver connected with a dali driver and lamp.
        Transmitter is disconnected with Mains
 
        Implement :
        Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
        Sleep 5*60s;
        Check the output of DALI, should send out Broadcast ARCP 254;

        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_6_1_C3_NoSignal_DALI_MA()
        {
            int sleepSec = 5 * 60 * 1000;
            byte rcvDaliData = 0;
            //Transmitter is disconnected with Mains
            MessageBox.Show("Please disconnect  transmitter with Mains");
            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);
            while (true)
            {
                target.Dali.StartSniffer("Dali_rx");
                target.Dali.StopSniffer("Dali_rx", 200);
                history.Clear();
                target.Dali.GetCmd("Dali_rx", DaliCommands.DAPC, 2, ref rcvDaliData);
                Flex.Log("DAPC" + rcvDaliData);
            }
            //Assert.AreEqual(254, rcvDaliData, "Fail for receiving ");
        }
        #endregion

        #region Cmrg1_6_1_C4_2Drivers_DALI
        [Test, Description(@"
            Narrative: Rx shall support two DALI drivers with one receiver.
 
            Precondition:
            Receiver connected with two DALI Ballasts and lamps.
 
            Implement :
            Set AC1Volt=230v, AC1Freq=50Hz, to AC1 power supply; 
            Set AC2Volt=5v, AC2Freq=50Hz*3, to AC2 power supply; 
            Sleep 20s
 
            Send CY1 CMD3 
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 254;
            Sleep 15s
 
            Send CY1 CMD4
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast OFF;
            Sleep 15s
 
            Send CY1 CMD5 
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 85;
            Sleep 15s
 
            Send CY1 CMD6
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 169;
            Sleep 15s
 
            Send CY1 CMD7
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 215;
            Sleep 15s
 
            Send CY1 CMD8 
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 229;
            Sleep 15s
 
            Send CY1 CMD9
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 235;
            Sleep 15s
 
 
            Send CY1 CMD10 
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 243;
            Sleep 15s
 
            Send CY1 CMD11 
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 245;
            Sleep 15s
 
            Send CY1 CMD12
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 247;
            Sleep 15s
 
            Send CY1 CMD13
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 250;
            Sleep 15s
 
            Send CY1 CMD14 
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 252;
            Sleep 15s
 
            Send CY1 CMD15
            Sleep 10s
            Get Dali output command from dali buster
            Check if useded command is broadcast DAPC and setting value is 254;
            Sleep 15s
        ")]
        [Category("Receiver(Manual)")]
        public void Cmrg1_6_1_C4_2Drivers_DALI()
        {
            UInt16 sleepSec = 20 * 1000;
            UInt16 _sleepSec = 15 * 1000;
            byte rcvDaliData = 0;
            //message box
            MessageBox.Show("please connect two dali drivers");
            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);
            IDaliObject[] objIdali = new IDaliObject[] {new Dali_GO_TO_SCENE_0(),new Dali_GO_TO_SCENE_1(),new Dali_GO_TO_SCENE_2(),
                                                        new Dali_GO_TO_SCENE_3(),new Dali_GO_TO_SCENE_4(),new Dali_GO_TO_SCENE_5(),
                                                        new Dali_GO_TO_SCENE_6(),new Dali_GO_TO_SCENE_7(),new Dali_GO_TO_SCENE_8(),
                                                        new Dali_GO_TO_SCENE_9(),new Dali_GO_TO_SCENE_10(),new Dali_GO_TO_SCENE_11(),
                                                        new Dali_GO_TO_SCENE_12()};
            for (ushort cmd = 3; cmd < 16; cmd++)
            {
                history.Clear();
                target.Dali.SendDaliCommand("Dali_tx", objIdali[cmd - 3]);

                Flex.Log("Send DALI Command is  " + cmd);
                target.Dali.StartSniffer("Dali_rx");
                target.Dali.StopSniffer("Dali_rx", 20000);

                target.Dali.GetCmd("Dali_rx", (cmd != 4) ? (DaliCommands.DIRECT_ARC_POWER_CONTROL) : (DaliCommands.OFF), 2, ref rcvDaliData);
                Flex.Log("DAPC valuse is " + rcvDaliData);

                UInt16[] ExpectValue = { 254, 0, 85, 169, 215, 229, 235, 243, 245, 247, 250, 252, 254 };
                Assert.AreEqual(ExpectValue[cmd - 3], rcvDaliData, "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(_sleepSec);
            }
        }
        #endregion

        #region Cmrg1_Test_Fullbridge
        [Test, Description(@"
        Test purpose
        ")]
        public void Cmrg1_Test_Fullbridge()
        {
            UInt16 cmdRcv = 0;
            int sleepSec = 10 * 1000;

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 0, 50);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (ushort cmd = 0; cmd < 16; cmd++)
            {
                history.Clear();
                // Set Relay board based on CY1 command
                if (Cmd2Dry(cmd) != 255)
                {
                    DrySwitch(Cmd2Dry(cmd));
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                    Flex.Log("Received CY1 CMD is " + cmdRcv);
                }
                else
                {
                    continue;
                }
                Thread.Sleep(5 * 60 * 1000);
            }

            target.Rxapp.Stop();
        }
        #endregion

        #region Cmrg1_Test_DALI
        [Test, Description(@"
        Test
        ")]
        public void Cmrg1_Test_DALI()
        {
            UInt16 cmdRcv = 0;
            ushort cmdPct = 0;
            int sleepSec = 30 * 1000;

            // Change AC1 mains to the given frequency
            target.AcPower.SetOutput("AC1", 230, 50);
            target.AcPower.SetOutput("AC2", 5, 50 * 3);
            Thread.Sleep(sleepSec);

            target.Rxapp.Start();

            for (int i = 0; i < 10; i++)
            {
                // test for dali
                for (ushort cmd = 3; cmd < 16; cmd++)
                {
                    history.Clear();
                    switch (cmd)
                    {
                        case 3:
                            IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 4:
                            objIdali = new Dali_GO_TO_SCENE_1();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 5:
                            objIdali = new Dali_GO_TO_SCENE_2();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 6:
                            objIdali = new Dali_GO_TO_SCENE_3();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 7:
                            objIdali = new Dali_GO_TO_SCENE_4();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 8:
                            objIdali = new Dali_GO_TO_SCENE_5();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 9:
                            objIdali = new Dali_GO_TO_SCENE_6();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 10:
                            objIdali = new Dali_GO_TO_SCENE_7();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 11:
                            objIdali = new Dali_GO_TO_SCENE_8();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 12:
                            objIdali = new Dali_GO_TO_SCENE_9();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 13:
                            objIdali = new Dali_GO_TO_SCENE_10();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 14:
                            objIdali = new Dali_GO_TO_SCENE_11();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 15:
                            objIdali = new Dali_GO_TO_SCENE_12();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                    }
                    Flex.Log("Send DALI Command is  " + cmd);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                    Flex.Log("Received DALI Command is  " + cmdRcv);
                    Assert.AreEqual(cmd, cmdRcv, "Fail for receiving CY1 CMD " + cmd);
                    target.Rxapp.Get_DimPercentage(out cmdPct, ref history, timeout: 20000);
                    Flex.Log("Percentage is  " + cmdPct);
                    Thread.Sleep(10 * 1000);
                }
                // test for dali
                for (ushort cmd1 = 14; cmd1 > 3; cmd1--)
                {
                    history.Clear();
                    switch (cmd1)
                    {
                        case 3:
                            IDaliObject objIdali = new Dali_GO_TO_SCENE_0();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 4:
                            objIdali = new Dali_GO_TO_SCENE_1();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 5:
                            objIdali = new Dali_GO_TO_SCENE_2();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 6:
                            objIdali = new Dali_GO_TO_SCENE_3();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 7:
                            objIdali = new Dali_GO_TO_SCENE_4();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 8:
                            objIdali = new Dali_GO_TO_SCENE_5();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 9:
                            objIdali = new Dali_GO_TO_SCENE_6();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 10:
                            objIdali = new Dali_GO_TO_SCENE_7();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 11:
                            objIdali = new Dali_GO_TO_SCENE_8();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 12:
                            objIdali = new Dali_GO_TO_SCENE_9();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 13:
                            objIdali = new Dali_GO_TO_SCENE_10();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 14:
                            objIdali = new Dali_GO_TO_SCENE_11();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                        case 15:
                            objIdali = new Dali_GO_TO_SCENE_12();
                            target.Dali.SendDaliCommand("Dali_tx", objIdali);
                            break;
                    }
                    Flex.Log("Send DALI Command is  " + cmd1);
                    target.Rxapp.Get_Cb_NewCmd(out cmdRcv, ref history, timeout: 20000);
                    Flex.Log("Received DALI Command is  " + cmdRcv);
                    Assert.AreEqual(cmd1, cmdRcv, "Fail for receiving CY1 CMD " + cmd1);
                    target.Rxapp.Get_DimPercentage(out cmdPct, ref history, timeout: 20000);
                    Flex.Log("Percentage is  " + cmdPct);
                    Thread.Sleep(10 * 1000);
                }
            }
            target.Rxapp.Stop();
        }
        #endregion


        #region CmTrg1_1_0_C1_DALIControl_EU
        [Test, Description(@"
        Narrative: Make a scheduler to control system based on dali commands. Observe if the behavior of Rxs are as expected.
 
        Precondition:
        Set up the system followed the picture attached on L-N folder.
        In this setup, receivers will be connected with one dali driver/  one 1-10v driver/ both dali drivers/ both 1-10v drivers.

        Implement :
        Set Jumper setting is L-N;
        Set AC1Volt=230v, AC1Freq=50Hz to AC1 power supply; 
        Set AC2Volt=0v, AC2Freq=50Hz to AC2 power supply;
        Sleep 30s;
       
        RXAPP_Start
        for loop
        RXAPP_Stop

        Set AC1Volt=230v, AC1Freq=50Hz to AC1 power supply; 
        Set AC2Volt=5v, AC2Freq=50Hz*3 to AC2 power supply;
        Sleep 30s;

        RXAPP_Start
        for loop
        RXAPP_Stop        
        ")]
        [Category("Transreceiver(AUTO)")]
        public void CmTrg1_1_0_C1_DALIControl_EU()
        {
            double volt_mains_1 = 230, volt_noise_1 = 0, volt_mains_2 = 230, volt_noise_2 = 5;
            double freq_mains_1 = 50, freq_noise_1 = 50, freq_mains_2 = 50, freq_noise_2 = 50 * 3;
            UInt16 Rcvcmd;
            short volt;
            byte rcvDaliData = 0;
            int SleepSec = 30 * 1000;

            IDaliObject[] objIdaliArray = new IDaliObject[13] {new Dali_GO_TO_SCENE_0(), new Dali_GO_TO_SCENE_1(), new Dali_GO_TO_SCENE_2(), 
                                                               new Dali_GO_TO_SCENE_3(), new Dali_GO_TO_SCENE_4(), new Dali_GO_TO_SCENE_5(), 
                                                               new Dali_GO_TO_SCENE_6(), new Dali_GO_TO_SCENE_7(), new Dali_GO_TO_SCENE_8(), 
                                                               new Dali_GO_TO_SCENE_9(), new Dali_GO_TO_SCENE_10(), new Dali_GO_TO_SCENE_11(),
                                                               new Dali_GO_TO_SCENE_12()};

            double[] Rx_volt_output = new double[13] {8000, 1000, 1000, 1000, 2939, 4100, 4900, 6049, 6398, 6798, 7197, 7596, 8000};

            UInt16[] Expected_ArcPower = { 254, 0, 85, 169, 215, 229, 235, 243, 245, 247, 250, 252, 254 };

            //Set Jumper to L-N
            target.Relay.Open("Dry5");

            // Set the mains and noise by the first given voltage and frequency
            target.AcPower.SetOutput("AC1", volt_mains_1, freq_mains_1);
            target.AcPower.SetOutput("AC2", volt_noise_1, freq_noise_1);
            Thread.Sleep(SleepSec);

            target.Rxapp.Start();

            for (ushort i = 0; i < 13; i++)
            {
                /*    Send "go to scene CY1CM" from DALI Buster;
                        Wait feedback of Rx while new command received;
                        Check if new command is equal to CY1CM;
                        Check if the output of Rx is equal to the scene defination of CY1CM;
                        Observe if other lamps behavior is same as monitored one;
                        Sleep 1h;
                        Observe if no change of Rx's output;
                */
                int cmd = i + 3;
                history.Clear();

                // Send "go to scene CY1CM" from DALI Buster;
                target.Dali.SendDaliCommand("Dali_tx",objIdaliArray[i]);
                Flex.Log("Sent dali command is " + cmd);
                Thread.Sleep(1000);
                // Wait feedback of Rx while new command received;
                target.Rxapp.Get_Cb_NewCmd(out Rcvcmd, ref history, timeout:20000);
                Flex.Log("Received command is " + Rcvcmd);
                Thread.Sleep(1000);
                // Check if new command is equal to CY1CM;
                Assert.AreEqual(cmd, Rcvcmd, "Fail for receiving CY1 CMD " + cmd);
                
               /* If new command = CY1CM, continue exectuing the following code. Else break */

                // Check if the output of Rx is equal to the scene defination of CY1CM;
                // Get 1-10V output of Rx
                target.Rxapp.Get_A1To10vOutput(out volt, ref history, timeout: 10000);
                // Check if the voltage output is within the expected range
                Assert.GreaterOrEqual(volt, Rx_volt_output[i] * (1 - 0.1), "Fail for receiving CY1 CMD " + cmd);
                Assert.LessOrEqual(volt, Rx_volt_output[i] * (1 + 0.1), "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10000);

                // Get the ArcPower output of Rx
                target.Dali.StartSniffer("Dali_rx");
                target.Dali.StopSniffer("Dali_rx", 20000);
                target.Dali.GetCmd("Dali_rx",(cmd != 4) ? (DaliCommands.DIRECT_ARC_POWER_CONTROL): (DaliCommands.OFF) ,2, ref rcvDaliData);
                Flex.Log("Dali_ArcPower value is " + rcvDaliData);

                Assert.AreEqual(Expected_ArcPower[i], rcvDaliData, "Fail for receiving CY1 CMD" + rcvDaliData);
                Thread.Sleep(15*1000);
                                                                            
                // Observe if other lamps behavior is same as monitored one 
                /* A message box with yes or no buttons shows " Do the other lamps behave the same as the menitored one?" */

                Thread.Sleep(60*60*1000); // Sleep 1h

                // Observe if no change of Rx's output
                /* To be continued */
            }

            target.Rxapp.Stop();

            // Set the mains and noise by the second given voltage and frequency
            target.AcPower.SetOutput("AC1", volt_mains_2, freq_mains_2);
            target.AcPower.SetOutput("AC2", volt_noise_2, freq_noise_2);
            Thread.Sleep(SleepSec);

            target.Rxapp.Start();

            for (ushort i = 0; i < 13; i++)
            {
                /*    Send "go to scene CY1CM" from DALI Buster;
                        Wait feedback of Rx while new command received;
                        Check if new command is equal to CY1CM;
                        Check if the output of Rx is equal to the scene defination of CY1CM;
                        Observe if other lamps behavior is same as monitored one;
                        Sleep 1h;
                        Observe if no change of Rx's output;
                */
                int cmd = i + 3;
                history.Clear();

                // Send "go to scene CY1CM" from DALI Buster;
                target.Dali.SendDaliCommand("Dali_tx", objIdaliArray[i]);
                Flex.Log("Sent dali command is " + cmd);
                Thread.Sleep(1000);
                // Wait feedback of Rx while new command received;
                target.Rxapp.Get_Cb_NewCmd(out Rcvcmd, ref history, timeout: 20000);
                Flex.Log("Received command is " + Rcvcmd);
                Thread.Sleep(1000);
                // Check if new command is equal to CY1CM;
                Assert.AreEqual(cmd, Rcvcmd, "Fail for receiving CY1 CMD " + cmd);

                /* If new command = CY1CM, continue exectuing the following code. Else break */

                // Check if the output of Rx is equal to the scene defination of CY1CM;
                // Get 1-10V output of Rx
                target.Rxapp.Get_A1To10vOutput(out volt, ref history, timeout: 10000);
                // Check if the voltage output is within the expected range
                Assert.GreaterOrEqual(volt, Rx_volt_output[i] * (1 - 0.1), "Fail for receiving CY1 CMD " + cmd);
                Assert.LessOrEqual(volt, Rx_volt_output[i] * (1 + 0.1), "Fail for receiving CY1 CMD " + cmd);
                Thread.Sleep(10000);

                // Get the ArcPower output of Rx
                target.Dali.StartSniffer("Dali_rx");
                target.Dali.StopSniffer("Dali_rx", 20000);
                target.Dali.GetCmd("Dali_rx", (cmd != 4) ? (DaliCommands.DIRECT_ARC_POWER_CONTROL) : (DaliCommands.OFF), 2, ref rcvDaliData);
                Flex.Log("Dali_ArcPower value is " + rcvDaliData);

                Assert.AreEqual(Expected_ArcPower[i], rcvDaliData, "Fail for receiving CY1 CMD" + rcvDaliData);
                Thread.Sleep(15 * 1000);

                // Observe if other lamps behavior is same as monitored one 
                /* A message box with yes or no buttons shows " Do the other lamps behave the same as the menitored one?" */

                Thread.Sleep(60 * 60 * 1000); // Sleep 1h

                // Observe if no change of Rx's output
                /* To be continued */
            }

            target.Rxapp.Stop();
        }
        #endregion       
    }
}


