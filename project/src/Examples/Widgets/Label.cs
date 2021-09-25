namespace via5
{
	class ExampleLabel : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.VerticalFlow();

			root_.ContentPanel.Add(new VUI.Label("label"));
			root_.ContentPanel.Add(new VUI.Label("bold", UnityEngine.FontStyle.Bold));

			var lb = new VUI.Label("green");
			lb.TextColor = new UnityEngine.Color(0, 1, 0);
			root_.ContentPanel.Add(lb);

			lb = new VUI.Label("monospace");
			lb.Font = VUI.Style.Theme.MonospaceFont;
			root_.ContentPanel.Add(lb);

			lb = new VUI.Label("large font");
			lb.FontSize = 40;
			root_.ContentPanel.Add(lb);
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
