namespace via5
{
	class ExampleTooltip : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();

			var label = new VUI.Label("mouse over this");
			label.Tooltip.Text = "this is a tooltip, only works on desktop";

			root_.ContentPanel.Add(label);
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
