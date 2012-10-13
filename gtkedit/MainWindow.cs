using System;
using System.IO;
using System.Text;
using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public string this_file_name = "";
	public bool file_saved = false;
	
	public void setWindowTitle(string title)
	{
		this.GdkWindow.Title = "GTKEdit> " + title;
	}
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		this.setWindowTitle("New file");
	}
	
	public bool QuitWithoutSaving()
	{
		MessageDialog md = new MessageDialog(this,
											 DialogFlags.Modal,
											 MessageType.Question,
											 ButtonsType.YesNo,
											 "File not saved\n Quit without saving?");
			
		ResponseType rt = (ResponseType) md.Run();
		
		md.Destroy();
		
		return (rt == ResponseType.Yes);
	}
	
	public bool try_to_write(string fname)
	{
		FileStream fs = null;
		
		try {
			fs = new FileStream(fname, FileMode.OpenOrCreate);
		} catch {
			Console.WriteLine("Failed to open file `" + fname + "`");
			return false;
		}
		
		if (fs.CanWrite == false)
			return false;
		
		byte[] bytearray = Encoding.UTF8.GetBytes(textview1.Buffer.Text);
		
		try {
			fs.Write(bytearray, 0, bytearray.Length);
			fs.Close();
		} catch {
			Console.WriteLine("Failed to write file `" + fname + "`");
			return false;
		}
		
		return true;
	}
	
	public bool try_to_read(string fname)
	{
		FileStream fs;
		try {
			fs = new FileStream(fname, FileMode.OpenOrCreate);
		} catch {
			Console.WriteLine("Failed to open file");
			return false;
		}
		
		if (fs.CanRead == false)
			return false;
		
		byte[] bytearray = new byte[fs.Length + 1];
		
		try {
			fs.Read(bytearray, 0, (int)fs.Length);
			textview1.Buffer.Text = Encoding.UTF8.GetString(bytearray);
			fs.Close();
		} catch {
			Console.WriteLine("Failed to read file");
			return false;			
		}
		
		return true;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Console.WriteLine("WindowClose clicked");

		bool quit_without_save = true;
		
		if (this.file_saved == false && textview1.Buffer.Modified == true)
			quit_without_save = this.QuitWithoutSaving();
		
		if (quit_without_save) {
			Console.WriteLine("Quitting...");
			Application.Quit();
		} else
			Console.WriteLine("Exit Canceled");

		a.RetVal = true;
	}

	protected void MenuFileClicked (object sender, System.EventArgs e)
	{
		Console.WriteLine("MenuFileClicked");
	}
	
	protected void MenuFileOpenClicked (object sender, System.EventArgs e)
	{	
		Console.WriteLine("MenuFileOpenClicked");
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("Choose file to open",
															 this,
															 FileChooserAction.Open,
															 "Cancel", ResponseType.Cancel,
															 "Open", ResponseType.Accept);
		
		if (fc.Run() == (int) ResponseType.Accept)
			this.this_file_name = fc.Filename;

		fc.Destroy();
		
		this.setWindowTitle(this.this_file_name);
		
		bool ok = this.try_to_read(this.this_file_name);
		
		if (ok)
			Console.WriteLine("Opened " + this.this_file_name);
		else
			Console.WriteLine("FAILED to open " + this.this_file_name);
	}

	protected void MenuFileSaveClicked (object sender, System.EventArgs e)
	{
		Console.WriteLine("MenuFileSaveClicked");
		
		if (this.this_file_name == "")
		{
			Console.WriteLine("New file choose name");
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("Choose file to save to",
															 this,
															 FileChooserAction.Save,
															 "Cancel", ResponseType.Cancel,
															 "Save", ResponseType.Accept);
		
			if (fc.Run() == (int) ResponseType.Accept)
			{	
				this.this_file_name = fc.Filename;
				fc.Destroy();
				this.setWindowTitle(this.this_file_name);
			}
			else
				return;
		}
		
		if ((this.file_saved = this.try_to_write(this.this_file_name)) == true)
			Console.WriteLine("Saved to " + this.this_file_name);
		else
			Console.WriteLine("Failed to save `" + this.this_file_name + "`");
	}

	protected void MenuFileSaveAsClicked (object sender, System.EventArgs e)
	{
		Console.WriteLine("MenuFileSaveAsClicked");
		
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("Choose file to save to",
															 this,
															 FileChooserAction.Save,
															 "Cancel", ResponseType.Cancel,
															 "Save", ResponseType.Accept);
		
		
		if (fc.Run() == (int) ResponseType.Accept)
		{
			string fileToSaveAs = fc.Filename;
			fc.Destroy();

			if (this.try_to_write(fileToSaveAs) == true)
				Console.WriteLine("Saved as " + fileToSaveAs);
			else
				Console.WriteLine("Failed to save as `" + fileToSaveAs + "`");
		}
	}

	protected void MenuFileCloseClicked (object sender, System.EventArgs e)
	{
		Console.WriteLine("WindowClose clicked");
		
		if (this.file_saved == false && textview1.Buffer.Modified == true)
		{
			if (this.QuitWithoutSaving() == false) {
				Console.WriteLine("Quit aborted");
				return;
			}
		}
		
		Console.WriteLine("Quitting...");
		Application.Quit();
	}
}
