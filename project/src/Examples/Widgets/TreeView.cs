namespace via5
{
	class ExampleTreeView : MVRScript
	{
		VUI.Root root_ = null;

		public override void Init()
		{
			root_ = new VUI.Root(this);

			root_.ContentPanel.Layout = new VUI.VerticalFlow();

			var tree = new VUI.TreeView();
			tree.MinimumSize = new VUI.Size(VUI.Widget.DontCare, 500);
			tree.SelectionChanged += (i) => SuperController.LogError(i.Text);

			for (int i = 0; i < 5; ++i)
			{
				var ii = new VUI.TreeView.Item($"item {i}");

				for (int j = 0; j < 4; ++j)
				{
					var ji = new VUI.TreeView.Item($"item {i}.{j}");

					for (int k = 0; k < 3; ++k)
						ji.Add(new VUI.TreeView.Item($"item {i}.{j}.{k}"));

					ii.Add(ji);
				}

				tree.RootItem.Add(ii);
			}

			root_.ContentPanel.Add(tree);
		}

		public void Update()
		{
			root_?.Update();
		}
	}
}
