using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mach3Extender {
	public partial class Form1 : Form {
		private Mach4.IMach4 mach = null;
		private Mach4.IMyScriptObject script = null;

		private string directory = "";

		// Hook into running Mach3 instance.
		// TODO: Detect if Mach3 is not running, then start it manually through referenced EXE
		public Form1() {
			InitializeComponent();

			directory = System.IO.Directory.GetCurrentDirectory();
			mach = (Mach4.IMach4) Marshal.GetActiveObject("Mach4.Document");
			script = (Mach4.IMyScriptObject) mach.GetScriptDispatch();
		}

		// When the Send Command button is pressed.
		private void SendCommandButton(object sender, EventArgs e) {
			if (textBox1.Text == "LOAD") { // If the command is LOAD
				//// A custom file format .m3ex is used to store extra information alongside the gcode.
				//BinaryFormatter formatter = new BinaryFormatter();
				//Hashtable data = null;

				//try {
				//	using (FileStream fs = File.OpenRead("C:\\Users\\myaka\\Documents\\Visual Studio 2015\\Projects\\Mach3Extender\\Mach3Extender\\bin\\Debug\\gcode\\final.m3ex")) {
				//		data = (Hashtable) formatter.Deserialize(fs);
				//	}

				//	using (FileStream fs = File.OpenWrite("C:\\Users\\myaka\\Documents\\Visual Studio 2015\\Projects\\Mach3Extender\\Mach3Extender\\bin\\Debug\\gcode\\final.temp.nc")) {
				//		byte[] gcode = (byte[]) data["gcode"];
				//		byte[] header = Encoding.ASCII.GetBytes("G92 X" + data["xOffset"] + " Y" + data["yOffset"] + " Z" + data["zOffset"] + "\n\rM0");

				//		fs.Write(header, 0, header.Length);
				//		fs.Write(gcode, 0, gcode.Length);
				//	}

				//	mach.LoadGCodeFile("C:\\Users\\myaka\\Documents\\Visual Studio 2015\\Projects\\Mach3Extender\\Mach3Extender\\bin\\Debug\\gcode\\final.temp.nc");
				//	//script.Code("G92 X" + data["xOffset"] + " Y" + data["yOffset"] + " Z" + data["zOffset"]);
				//	Thread.Sleep(2000);
				//	mach.CycleStart();
				//} catch (Exception ex) {

				//}

				//mach.LoadGCodeFile("C:\\Users\\myaka\\Documents\\Visual Studio 2015\\Projects\\Mach3Extender\\Mach3Extender\\bin\\Debug\\gcode\\test.m3ex");
			} else if (textBox1.Text == "START") {
				mach.CycleStart();
			} else {
				script.Code(textBox1.Text);
			}
		}

		private void loadgcode_Click(object sender, EventArgs e) {
			openFileDialog1.ShowDialog();
		}

		private void LoadM3EX(Hashtable data) {
			try {
				string directory = this.directory + "\\working_gcode.nc";

				// Write a temp gcode file to the directory we are in. 
				using (FileStream fs = File.OpenWrite(directory)) {
					byte[] gcode = (byte[]) data["gcode"];

					// We insert some "header" gcode to do a few things setup-wise inside of Mach3 since not all things can be done via script command.
					byte[] header = Encoding.ASCII.GetBytes(
						"G10 L2 P1 X" + data["xOffset"] + " Y" + data["yOffset"] + " Z" + data["zOffset"] + // Set the offsets
						"\nG00 Z0" + // Set position of tool
						"\nG00 X0 Y0" + // Set position of tool
						"\nM0"); // Pause command

					// Write the header and toolpath gcode to the temporary gcode file.
					fs.Write(header, 0, header.Length);
					fs.Write(gcode, 0, gcode.Length);
				}

				// Load up our gcode file.
				mach.LoadGCodeFile(directory);
				
				// Give 2 seconds for Mach3 to load (this can be configured).
				Thread.Sleep(2000);

				// Start the cycle.
				mach.CycleStart();
			} catch (Exception e) {

			}
		}

		// Load up an .m3ex file
		private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
			if (File.Exists(openFileDialog1.FileName)) {
				Hashtable data = null;

				try {
					BinaryFormatter formatter = new BinaryFormatter();

					using (FileStream fs = File.OpenRead(openFileDialog1.FileName)) {
						// .m3ex is simply a serialization of the gcode plus other various variables used by this program and Mach3.
						data = (Hashtable) formatter.Deserialize(fs);
					}

				} catch (Exception ex) {
					return;
				}
				
				LoadM3EX(data);
			}
		}
	}
}
