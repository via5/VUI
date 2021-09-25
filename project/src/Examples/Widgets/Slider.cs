namespace via5
{
	class ExampleSlider : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			var p = new VUI.Panel(new VUI.GridLayout(2));

			p.Add(new VUI.Label("float"));
			p.Add(new VUI.FloatSlider(0, 10, (v) => SuperController.LogError($"{v}")));

			p.Add(new VUI.Label("int"));
			p.Add(new VUI.IntSlider(0, 10, (v) => SuperController.LogError($"{v}")));

			p.Add(new VUI.Label("float with text"));
			p.Add(new VUI.FloatTextSlider(0, 10, (v) => SuperController.LogError($"{v}")));

			p.Add(new VUI.Label("int with text"));
			p.Add(new VUI.IntTextSlider(0, 10, (v) => SuperController.LogError($"{v}")));

			root_.ContentPanel.Layout = new VUI.HorizontalFlow();
			root_.ContentPanel.Add(p);
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
