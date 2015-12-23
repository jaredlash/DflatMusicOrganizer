
// This file has been generated by the GUI designer. Do not modify.
public partial class MainWindow
{
	private global::Gtk.VBox vbox1;
	private global::Gtk.Notebook notebook1;
	private global::Gtk.Label label5;
	private global::Gtk.Label label1;
	private global::Gtk.HPaned hpaned1;
	private global::Gtk.VBox vbox2;
	private global::Gtk.HButtonBox hbuttonbox1;
	private global::Gtk.Button button1;
	private global::Gtk.Button button2;
	private global::Gtk.Button button115;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.TreeView treeview1;
	private global::Gtk.HButtonBox hbuttonbox2;
	private global::Gtk.Button button4;
	private global::Gtk.Button button116;
	private global::Gtk.VBox vbox3;
	private global::Gtk.HBox hbox1;
	private global::Gtk.HButtonBox hbuttonbox3;
	private global::Gtk.Button button5;
	private global::Gtk.Button button6;
	private global::Gtk.VSeparator vseparator1;
	private global::Gtk.HButtonBox hbuttonbox4;
	private global::Gtk.Button button11;
	private global::Gtk.Button button12;
	private global::Gtk.ScrolledWindow GtkScrolledWindow1;
	private global::Gtk.TreeView treeview2;
	private global::Gtk.HBox hbox2;
	private global::Gtk.Label label6;
	private global::Gtk.Label label7;
	private global::Gtk.HButtonBox hbuttonbox5;
	private global::Gtk.Button button13;
	private global::Gtk.Button button14;
	private global::Gtk.Label label2;
	private global::Gtk.Label label4;
	private global::Gtk.Label label3;
	private global::Gtk.Statusbar statusbar1;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.notebook1 = new global::Gtk.Notebook ();
		this.notebook1.CanFocus = true;
		this.notebook1.Name = "notebook1";
		this.notebook1.CurrentPage = 1;
		// Container child notebook1.Gtk.Notebook+NotebookChild
		this.label5 = new global::Gtk.Label ();
		this.label5.Name = "label5";
		this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("music library temp label");
		this.notebook1.Add (this.label5);
		// Notebook tab
		this.label1 = new global::Gtk.Label ();
		this.label1.Name = "label1";
		this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Music Library");
		this.notebook1.SetTabLabel (this.label5, this.label1);
		this.label1.ShowAll ();
		// Container child notebook1.Gtk.Notebook+NotebookChild
		this.hpaned1 = new global::Gtk.HPaned ();
		this.hpaned1.CanFocus = true;
		this.hpaned1.Name = "hpaned1";
		this.hpaned1.Position = 310;
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 6;
		// Container child vbox2.Gtk.Box+BoxChild
		this.hbuttonbox1 = new global::Gtk.HButtonBox ();
		this.hbuttonbox1.Name = "hbuttonbox1";
		// Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
		this.button1 = new global::Gtk.Button ();
		this.button1.CanFocus = true;
		this.button1.Name = "button1";
		this.button1.UseUnderline = true;
		// Container child button1.Gtk.Container+ContainerChild
		global::Gtk.Alignment w2 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w3 = new global::Gtk.HBox ();
		w3.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w4 = new global::Gtk.Image ();
		w4.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-back", global::Gtk.IconSize.Menu);
		w3.Add (w4);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w6 = new global::Gtk.Label ();
		w6.LabelProp = global::Mono.Unix.Catalog.GetString ("_Back");
		w6.UseUnderline = true;
		w3.Add (w6);
		w2.Add (w3);
		this.button1.Add (w2);
		this.hbuttonbox1.Add (this.button1);
		global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1 [this.button1]));
		w10.Expand = false;
		w10.Fill = false;
		// Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
		this.button2 = new global::Gtk.Button ();
		this.button2.CanFocus = true;
		this.button2.Name = "button2";
		this.button2.UseUnderline = true;
		// Container child button2.Gtk.Container+ContainerChild
		global::Gtk.Alignment w11 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w12 = new global::Gtk.HBox ();
		w12.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w13 = new global::Gtk.Image ();
		w13.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-up", global::Gtk.IconSize.Menu);
		w12.Add (w13);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w15 = new global::Gtk.Label ();
		w15.LabelProp = global::Mono.Unix.Catalog.GetString ("_Up a level");
		w15.UseUnderline = true;
		w12.Add (w15);
		w11.Add (w12);
		this.button2.Add (w11);
		this.hbuttonbox1.Add (this.button2);
		global::Gtk.ButtonBox.ButtonBoxChild w19 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1 [this.button2]));
		w19.Position = 1;
		w19.Expand = false;
		w19.Fill = false;
		// Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
		this.button115 = new global::Gtk.Button ();
		this.button115.CanFocus = true;
		this.button115.Name = "button115";
		this.button115.UseUnderline = true;
		// Container child button115.Gtk.Container+ContainerChild
		global::Gtk.Alignment w20 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w21 = new global::Gtk.HBox ();
		w21.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w22 = new global::Gtk.Image ();
		w22.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-find", global::Gtk.IconSize.Menu);
		w21.Add (w22);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w24 = new global::Gtk.Label ();
		w24.LabelProp = global::Mono.Unix.Catalog.GetString ("Filter");
		w24.UseUnderline = true;
		w21.Add (w24);
		w20.Add (w21);
		this.button115.Add (w20);
		this.hbuttonbox1.Add (this.button115);
		global::Gtk.ButtonBox.ButtonBoxChild w28 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1 [this.button115]));
		w28.Position = 2;
		w28.Expand = false;
		w28.Fill = false;
		this.vbox2.Add (this.hbuttonbox1);
		global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbuttonbox1]));
		w29.Position = 0;
		w29.Expand = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.treeview1 = new global::Gtk.TreeView ();
		this.treeview1.CanFocus = true;
		this.treeview1.Name = "treeview1";
		this.GtkScrolledWindow.Add (this.treeview1);
		this.vbox2.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.GtkScrolledWindow]));
		w31.Position = 1;
		// Container child vbox2.Gtk.Box+BoxChild
		this.hbuttonbox2 = new global::Gtk.HButtonBox ();
		this.hbuttonbox2.Name = "hbuttonbox2";
		// Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
		this.button4 = new global::Gtk.Button ();
		this.button4.CanFocus = true;
		this.button4.Name = "button4";
		this.button4.UseUnderline = true;
		// Container child button4.Gtk.Container+ContainerChild
		global::Gtk.Alignment w32 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w33 = new global::Gtk.HBox ();
		w33.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w34 = new global::Gtk.Image ();
		w34.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-add", global::Gtk.IconSize.Menu);
		w33.Add (w34);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w36 = new global::Gtk.Label ();
		w36.LabelProp = global::Mono.Unix.Catalog.GetString ("Add Music Source");
		w36.UseUnderline = true;
		w33.Add (w36);
		w32.Add (w33);
		this.button4.Add (w32);
		this.hbuttonbox2.Add (this.button4);
		global::Gtk.ButtonBox.ButtonBoxChild w40 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2 [this.button4]));
		w40.Expand = false;
		w40.Fill = false;
		// Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
		this.button116 = new global::Gtk.Button ();
		this.button116.CanFocus = true;
		this.button116.Name = "button116";
		this.button116.UseUnderline = true;
		// Container child button116.Gtk.Container+ContainerChild
		global::Gtk.Alignment w41 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w42 = new global::Gtk.HBox ();
		w42.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w43 = new global::Gtk.Image ();
		w43.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-refresh", global::Gtk.IconSize.Menu);
		w42.Add (w43);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w45 = new global::Gtk.Label ();
		w45.LabelProp = global::Mono.Unix.Catalog.GetString ("Scan Sources");
		w45.UseUnderline = true;
		w42.Add (w45);
		w41.Add (w42);
		this.button116.Add (w41);
		this.hbuttonbox2.Add (this.button116);
		global::Gtk.ButtonBox.ButtonBoxChild w49 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2 [this.button116]));
		w49.Position = 1;
		w49.Expand = false;
		w49.Fill = false;
		this.vbox2.Add (this.hbuttonbox2);
		global::Gtk.Box.BoxChild w50 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbuttonbox2]));
		w50.Position = 2;
		w50.Expand = false;
		w50.Fill = false;
		this.hpaned1.Add (this.vbox2);
		global::Gtk.Paned.PanedChild w51 = ((global::Gtk.Paned.PanedChild)(this.hpaned1 [this.vbox2]));
		w51.Resize = false;
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.hbuttonbox3 = new global::Gtk.HButtonBox ();
		this.hbuttonbox3.Name = "hbuttonbox3";
		// Container child hbuttonbox3.Gtk.ButtonBox+ButtonBoxChild
		this.button5 = new global::Gtk.Button ();
		this.button5.CanFocus = true;
		this.button5.Name = "button5";
		this.button5.UseUnderline = true;
		// Container child button5.Gtk.Container+ContainerChild
		global::Gtk.Alignment w52 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w53 = new global::Gtk.HBox ();
		w53.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w54 = new global::Gtk.Image ();
		w54.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-add", global::Gtk.IconSize.Menu);
		w53.Add (w54);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w56 = new global::Gtk.Label ();
		w56.LabelProp = global::Mono.Unix.Catalog.GetString ("Mark selected");
		w56.UseUnderline = true;
		w53.Add (w56);
		w52.Add (w53);
		this.button5.Add (w52);
		this.hbuttonbox3.Add (this.button5);
		global::Gtk.ButtonBox.ButtonBoxChild w60 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox3 [this.button5]));
		w60.Expand = false;
		w60.Fill = false;
		// Container child hbuttonbox3.Gtk.ButtonBox+ButtonBoxChild
		this.button6 = new global::Gtk.Button ();
		this.button6.CanFocus = true;
		this.button6.Name = "button6";
		this.button6.UseUnderline = true;
		// Container child button6.Gtk.Container+ContainerChild
		global::Gtk.Alignment w61 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w62 = new global::Gtk.HBox ();
		w62.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w63 = new global::Gtk.Image ();
		w63.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-remove", global::Gtk.IconSize.Menu);
		w62.Add (w63);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w65 = new global::Gtk.Label ();
		w65.LabelProp = global::Mono.Unix.Catalog.GetString ("Unmark selected");
		w65.UseUnderline = true;
		w62.Add (w65);
		w61.Add (w62);
		this.button6.Add (w61);
		this.hbuttonbox3.Add (this.button6);
		global::Gtk.ButtonBox.ButtonBoxChild w69 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox3 [this.button6]));
		w69.Position = 1;
		w69.Expand = false;
		w69.Fill = false;
		this.hbox1.Add (this.hbuttonbox3);
		global::Gtk.Box.BoxChild w70 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.hbuttonbox3]));
		w70.Position = 0;
		w70.Expand = false;
		w70.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.vseparator1 = new global::Gtk.VSeparator ();
		this.vseparator1.Name = "vseparator1";
		this.hbox1.Add (this.vseparator1);
		global::Gtk.Box.BoxChild w71 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vseparator1]));
		w71.Position = 1;
		w71.Expand = false;
		w71.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.hbuttonbox4 = new global::Gtk.HButtonBox ();
		this.hbuttonbox4.Name = "hbuttonbox4";
		// Container child hbuttonbox4.Gtk.ButtonBox+ButtonBoxChild
		this.button11 = new global::Gtk.Button ();
		this.button11.CanFocus = true;
		this.button11.Name = "button11";
		this.button11.UseUnderline = true;
		// Container child button11.Gtk.Container+ContainerChild
		global::Gtk.Alignment w72 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w73 = new global::Gtk.HBox ();
		w73.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w74 = new global::Gtk.Image ();
		w74.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-yes", global::Gtk.IconSize.Menu);
		w73.Add (w74);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w76 = new global::Gtk.Label ();
		w76.LabelProp = global::Mono.Unix.Catalog.GetString ("Mark all");
		w76.UseUnderline = true;
		w73.Add (w76);
		w72.Add (w73);
		this.button11.Add (w72);
		this.hbuttonbox4.Add (this.button11);
		global::Gtk.ButtonBox.ButtonBoxChild w80 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox4 [this.button11]));
		w80.Expand = false;
		w80.Fill = false;
		// Container child hbuttonbox4.Gtk.ButtonBox+ButtonBoxChild
		this.button12 = new global::Gtk.Button ();
		this.button12.CanFocus = true;
		this.button12.Name = "button12";
		this.button12.UseUnderline = true;
		// Container child button12.Gtk.Container+ContainerChild
		global::Gtk.Alignment w81 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w82 = new global::Gtk.HBox ();
		w82.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w83 = new global::Gtk.Image ();
		w83.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-no", global::Gtk.IconSize.Menu);
		w82.Add (w83);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w85 = new global::Gtk.Label ();
		w85.LabelProp = global::Mono.Unix.Catalog.GetString ("Unmark all");
		w85.UseUnderline = true;
		w82.Add (w85);
		w81.Add (w82);
		this.button12.Add (w81);
		this.hbuttonbox4.Add (this.button12);
		global::Gtk.ButtonBox.ButtonBoxChild w89 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox4 [this.button12]));
		w89.Position = 1;
		w89.Expand = false;
		w89.Fill = false;
		this.hbox1.Add (this.hbuttonbox4);
		global::Gtk.Box.BoxChild w90 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.hbuttonbox4]));
		w90.Position = 2;
		w90.Expand = false;
		w90.Fill = false;
		this.vbox3.Add (this.hbox1);
		global::Gtk.Box.BoxChild w91 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
		w91.Position = 0;
		w91.Expand = false;
		w91.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		this.treeview2 = new global::Gtk.TreeView ();
		this.treeview2.CanFocus = true;
		this.treeview2.Name = "treeview2";
		this.GtkScrolledWindow1.Add (this.treeview2);
		this.vbox3.Add (this.GtkScrolledWindow1);
		global::Gtk.Box.BoxChild w93 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow1]));
		w93.Position = 1;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox2 = new global::Gtk.HBox ();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.label6 = new global::Gtk.Label ();
		this.label6.Name = "label6";
		this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Marked: ");
		this.hbox2.Add (this.label6);
		global::Gtk.Box.BoxChild w94 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.label6]));
		w94.Position = 0;
		w94.Expand = false;
		w94.Fill = false;
		// Container child hbox2.Gtk.Box+BoxChild
		this.label7 = new global::Gtk.Label ();
		this.label7.Name = "label7";
		this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("1,337");
		this.hbox2.Add (this.label7);
		global::Gtk.Box.BoxChild w95 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.label7]));
		w95.Position = 1;
		w95.Expand = false;
		w95.Fill = false;
		// Container child hbox2.Gtk.Box+BoxChild
		this.hbuttonbox5 = new global::Gtk.HButtonBox ();
		this.hbuttonbox5.Name = "hbuttonbox5";
		// Container child hbuttonbox5.Gtk.ButtonBox+ButtonBoxChild
		this.button13 = new global::Gtk.Button ();
		this.button13.CanFocus = true;
		this.button13.Name = "button13";
		this.button13.UseUnderline = true;
		// Container child button13.Gtk.Container+ContainerChild
		global::Gtk.Alignment w96 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w97 = new global::Gtk.HBox ();
		w97.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w98 = new global::Gtk.Image ();
		w98.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-apply", global::Gtk.IconSize.Menu);
		w97.Add (w98);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w100 = new global::Gtk.Label ();
		w100.LabelProp = global::Mono.Unix.Catalog.GetString ("Organize Marked Items");
		w100.UseUnderline = true;
		w97.Add (w100);
		w96.Add (w97);
		this.button13.Add (w96);
		this.hbuttonbox5.Add (this.button13);
		global::Gtk.ButtonBox.ButtonBoxChild w104 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox5 [this.button13]));
		w104.Expand = false;
		w104.Fill = false;
		// Container child hbuttonbox5.Gtk.ButtonBox+ButtonBoxChild
		this.button14 = new global::Gtk.Button ();
		this.button14.CanFocus = true;
		this.button14.Name = "button14";
		this.button14.UseUnderline = true;
		// Container child button14.Gtk.Container+ContainerChild
		global::Gtk.Alignment w105 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w106 = new global::Gtk.HBox ();
		w106.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w107 = new global::Gtk.Image ();
		w107.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-edit", global::Gtk.IconSize.Menu);
		w106.Add (w107);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w109 = new global::Gtk.Label ();
		w109.LabelProp = global::Mono.Unix.Catalog.GetString ("Edit Tags");
		w109.UseUnderline = true;
		w106.Add (w109);
		w105.Add (w106);
		this.button14.Add (w105);
		this.hbuttonbox5.Add (this.button14);
		global::Gtk.ButtonBox.ButtonBoxChild w113 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox5 [this.button14]));
		w113.Position = 1;
		w113.Expand = false;
		w113.Fill = false;
		this.hbox2.Add (this.hbuttonbox5);
		global::Gtk.Box.BoxChild w114 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.hbuttonbox5]));
		w114.PackType = ((global::Gtk.PackType)(1));
		w114.Position = 2;
		w114.Expand = false;
		w114.Fill = false;
		this.vbox3.Add (this.hbox2);
		global::Gtk.Box.BoxChild w115 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox2]));
		w115.Position = 2;
		w115.Expand = false;
		w115.Fill = false;
		this.hpaned1.Add (this.vbox3);
		this.notebook1.Add (this.hpaned1);
		global::Gtk.Notebook.NotebookChild w117 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.hpaned1]));
		w117.Position = 1;
		// Notebook tab
		this.label2 = new global::Gtk.Label ();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Organizer");
		this.notebook1.SetTabLabel (this.hpaned1, this.label2);
		this.label2.ShowAll ();
		// Container child notebook1.Gtk.Notebook+NotebookChild
		this.label4 = new global::Gtk.Label ();
		this.label4.Name = "label4";
		this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Job scheduler temp label");
		this.notebook1.Add (this.label4);
		global::Gtk.Notebook.NotebookChild w118 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.label4]));
		w118.Position = 2;
		// Notebook tab
		this.label3 = new global::Gtk.Label ();
		this.label3.Name = "label3";
		this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Job Scheduler");
		this.notebook1.SetTabLabel (this.label4, this.label3);
		this.label3.ShowAll ();
		this.vbox1.Add (this.notebook1);
		global::Gtk.Box.BoxChild w119 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.notebook1]));
		w119.Position = 0;
		// Container child vbox1.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.vbox1.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w120 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.statusbar1]));
		w120.Position = 1;
		w120.Expand = false;
		w120.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 884;
		this.DefaultHeight = 300;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}
