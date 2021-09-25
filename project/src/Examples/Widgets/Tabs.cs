namespace via5
{
	class ExampleTabs : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			var tabs = new VUI.Tabs();

			for (int i = 1; i <= 3; ++i)
			{
				var p = new VUI.Panel(new VUI.HorizontalFlow());

				p.Add(new VUI.Button(
					$"button in tab {i}",
					() => SuperController.LogError("clicked")));

				tabs.AddTab($"tab {i}", p);
			}

			root_.ContentPanel.Layout = new VUI.BorderLayout();
			root_.ContentPanel.Add(tabs, VUI.BorderLayout.Center);
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
