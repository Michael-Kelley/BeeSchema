using System.Collections.ObjectModel;

namespace BeeSchema {
	public class ResultCollection : KeyedCollection<string, Result> {
		protected override string GetKeyForItem(Result item)
			=> item.Name;
	}
}