namespace ClientWF
{
	public partial class Form1 : Form
	{
		public Form1() => InitializeComponent();

		private void Form1_Load(object sender, EventArgs e)
		{
			this.ShowInTaskbar = false;
			this.Opacity = 0;
			this.Location = new Point(-999999, -999999);
		}


	}
}