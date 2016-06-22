using System.Collections.Generic;

namespace BeeSchema {
	public class Node {
		public NodeType Type { get; internal set; }
		public string Name { get; internal set; }
		public string Comment { get; internal set; }
		public List<Node> Children { get; internal set; }
		public object Value { get; internal set; }

		public Node() {
			Children = new List<Node>();
		}
	}
}