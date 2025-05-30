using HidSharp;
using HidSharp.Reports;
using HidSharp.Utility;
using System.Diagnostics;

// Windows .NET Publishing notes
// AOT: dotnet publish -r win-x64 -c Release /p:PublishAot=true
// Jit: dotnet publish -r win-x64 -c Release

// macOS .NET Publishing notes
// AOT: dotnet publish -r osx-x64|osx-arm64 -c Release /p:PublishAot=true
// Jit: dotnet publish -r osx-x64|osx-arm64 -c Release

// Linux .NET Publishing notes
// AOT: dotnet publish -r linux-x64 -c Release /p:PublishAot=true
// Jit: dotnet publish -r linux-x64 -c Release

namespace HidTester
{
    enum Mode : byte
    {
        Offline = 0,
        XInput = 1,
        DInput = 2,
        MSI = 3,
        Desktop = 4,
        BIOS = 5,
        Testing = 6
    }

    internal class Program
	{
        static int vid = 0x0DB0;
        static int pid = 0x1901;
        static int readDelay = 100;
        static byte[] dataToWrite = new byte[8];
        static byte[] dataToRead = new byte[1024];

        static void Main(string[] args)
		{
            Log.WriteLine("<< HID Tester >>");

            // fill default data for MSI-Claw
            int i = 0;
            dataToWrite[i++] = 0x0F;// (aka 15);// report id
            dataToWrite[i++] = 0;
            dataToWrite[i++] = 0;
            dataToWrite[i++] = 0x3C;// (aka 60);
            dataToWrite[i++] = 0x24;// (aka 36);// we want to switch mode
            dataToWrite[i++] = (byte)Mode.XInput;// mode
            dataToWrite[i++] = 0;
            dataToWrite[i++] = 0;

            // process args
            if (args != null)
            {
                foreach (string arg in args)
                {
                    if (arg.StartsWith("--help"))
                    {
                        Log.WriteLine("-vid={dec|hex}");
                        Log.WriteLine("-pid={dec|hex}");
                        Log.WriteLine("-read-delay={ms}");
                        Log.WriteLine("-data-file={text-file-path} [each byte on its own line]");
                        return;
                    }
                    else if (arg.StartsWith("-vid="))
                    {
                        var parts = arg.Split('=');
                        if (int.TryParse(parts[1], out int value)) vid = value;
                        else vid = Convert.ToInt32(parts[1], 16);
                    }
                    else if (arg.StartsWith("-pid="))
                    {
                        var parts = arg.Split('=');
                        if (int.TryParse(parts[1], out int value)) pid = value;
                        else pid = Convert.ToInt32(parts[1], 16);
                    }
                    else if (arg.StartsWith("-read-delay="))
                    {
                        var parts = arg.Split('=');
                        if (int.TryParse(parts[1], out int value)) readDelay = value;
                    }
                    else if (arg.StartsWith("-data-file="))
                    {
                        var buffer = new List<byte>();
                        var parts = arg.Split('=');
                        using (var reader = new StreamReader(parts[1]))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (byte.TryParse(line, out byte value)) buffer.Add(value);
                                else buffer.Add(Convert.ToByte(line, 16));
                            }
                        }
                    }
                }
            }

            // log args
            Log.WriteLine($"Using Device: VID=0x{vid.ToString("X")} PID=0x{pid.ToString("X")}");
            Log.WriteLine($"Read after delay of {readDelay}ms");

            // get all devices
            Log.WriteLine();
            Log.WriteLine("Searching for HID devices...");
            HidSharpDiagnostics.EnableTracing = true;
            HidSharpDiagnostics.PerformStrictChecks = true;
			var devices = DeviceList.Local.GetHidDevices(vid, pid);
			foreach (var device in devices)
			{
				try
				{
                    Log.WriteLine();
                    Log.WriteLine();
                    WriteDevice(device);
				}
				catch (Exception e)
				{
                    Log.WriteLine(e);
				}
			}
		}

		static void WriteDevice(HidDevice device)
		{
            // get device desc
            Log.WriteLine($"Getting Device desc for: {device.DevicePath}");
            var desc = device.GetReportDescriptor();
            foreach (var report in desc.Reports)
            {
                Log.WriteLine($"ReportID:{report.ReportID} ReportType:{report.ReportType}");
            }
            Log.WriteLine();

            // open device
            Log.WriteLine("Opening HID stream...");
            if (!device.TryOpen(out var hidStream))
            {
                Log.WriteLine("Failed to open HID Stream");
                return;
            }
            else
            {
                Log.WriteLine("Open HID stream Success!");
            }

            using (hidStream)
            {
                if (!hidStream.CanWrite)
                {
                    Log.WriteLine("Can't write");
                    return;
                }

                hidStream.ReadTimeout = 5000;// 5 seconds
                hidStream.WriteTimeout = 5000;// 5 seconds

                // test stuff
                //var inputReceiver = desc.CreateHidDeviceInputReceiver();

                // write gamepad for xinput
                Log.WriteLine("Writing data...");
                hidStream.Write(dataToWrite);

                // wait before read
                Thread.Sleep(readDelay);

                // read response
                int read = hidStream.Read(dataToRead);
                if (read > 0)
                {
                    Log.WriteLine($"Read data size of {read}");
                    for (int i = 0; i != read; ++i)
                    {
                        byte b = dataToRead[i];
                        Log.WriteLine("0x" + b.ToString("X"));
                    }
                }
                else
                {
                    Log.WriteLine("No data read");
                }
            }
        }

    }
}
