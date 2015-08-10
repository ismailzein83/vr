using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierGroupNode
    {
        public int EntityId { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public CarrierGroupNode Parent { get; set; }

        public List<CarrierGroupNode> Children { get; set; }

    }


    public static class BusinessEntityHelper
    {
        public static IEnumerable<CarrierGroupNode> Descendants(this CarrierGroupNode root)
        {
            var nodes = new Stack<CarrierGroupNode>(new[] { root });
            while (nodes.Any())
            {
                CarrierGroupNode node = nodes.Pop();
                yield return node;
                if (node.Children != null)
                    foreach (var n in node.Children) nodes.Push(n);
            }
        }

        public static string GetRelativePath(this CarrierGroupNode node)
        {
            List<string> listofNames = new List<string>();

            while (node.Parent != null)
            {
                listofNames.Add(node.Name);
                node = node.Parent;
            }

            listofNames.Add(node.Name);
            listofNames.Reverse();

            StringBuilder path = new StringBuilder();

            foreach (string item in listofNames)
            {
                path.AppendFormat("{0}/", item);
            }

            return path.ToString().TrimEnd('/');
        }
    }

}
