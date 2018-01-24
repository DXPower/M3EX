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

		public Form1() {
			InitializeComponent();

			directory = System.IO.Directory.GetCurrentDirectory();
			mach = (Mach4.IMach4) Marshal.GetActiveObject("Mach4.Document");
			script = (Mach4.IMyScriptObject) mach.GetScriptDispatch();
		}

		private void button1_Click(object sender, EventArgs e) {
			if (textBox1.Text == "LOAD") {
				BinaryFormatter formatter = new BinaryFormatter();
				Hashtable data = null;
				


				try {
					using (FileStream fs = File.OpenRead("C:\\Users\\myaka\\Documents\\Visual Studio 2015\\Projects\\Mach3Extender\\Mach3Extender\\bin\\Debug\\gcode\\final.m3ex")) {
						data = (Hashtable) formatter.Deserialize(fs);
					}

					using (FileStream fs = File.OpenWrite("C:\\Users\\myaka\\Documents\\Visual Studio 2015\\Projects\\Mach3Extender\\Mach3Extender\\bin\\Debug\\gcode\\final.temp.nc")) {
						byte[] gcode = (byte[]) data["gcode"];
						byte[] header = Encoding.ASCII.GetBytes("G92 X" + data["xOffset"] + " Y" + data["yOffset"] + " Z" + data["zOffset"] + "\n\rM0");

						fs.Write(header, 0, header.Length);
						fs.Write(gcode, 0, gcode.Length);
					}

					mach.LoadGCodeFile("C:\\Users\\myaka\\Documents\\Visual Studio 2015\\Projects\\Mach3Extender\\Mach3Extender\\bin\\Debug\\gcode\\final.temp.nc");
					//script.Code("G92 X" + data["xOffset"] + " Y" + data["yOffset"] + " Z" + data["zOffset"]);
					Thread.Sleep(2000);
					mach.CycleStart();
				} catch (Exception ex) {

				}


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

				using (FileStream fs = File.OpenWrite(directory)) {
					byte[] gcode = (byte[]) data["gcode"];
					byte[] header = Encoding.ASCII.GetBytes(
						"G10 L2 P1 X" + data["xOffset"] + " Y" + data["yOffset"] + " Z" + data["zOffset"] + 
						"\nG00 Z0" +
						"\nG00 X0 Y0" +
						"\nM0");

					fs.Write(header, 0, header.Length);
					fs.Write(gcode, 0, gcode.Length);
				}

				mach.LoadGCodeFile(directory);
				
				Thread.Sleep(2000);
				mach.CycleStart();
			} catch (Exception e) {

			}
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
			if (File.Exists(openFileDialog1.FileName)) {
				Hashtable data = null;

				try {
					BinaryFormatter formatter = new BinaryFormatter();

					using (FileStream fs = File.OpenRead(openFileDialog1.FileName)) {
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
