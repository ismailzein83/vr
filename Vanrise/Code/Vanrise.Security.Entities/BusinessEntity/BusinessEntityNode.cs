using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class BusinessEntityNode
    {
        public Guid EntityId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public EntityType EntType { get; set; }

        public List<string> PermissionOptions { get; set; }

        public bool BreakInheritance { get; set; }

        [JsonIgnore]
        public BusinessEntityNode Parent { get; set; }

        public List<BusinessEntityNode> Children { get; set; }

    }

    public enum EntityType
    {
        MODULE,
        ENTITY
    }

    public static class BusinessEntityHelper
    {
        public static IEnumerable<BusinessEntityNode> Descendants(this BusinessEntityNode root)
        {
            var nodes = new Stack<BusinessEntityNode>(new[] { root });
            while (nodes.Any())
            {
                BusinessEntityNode node = nodes.Pop();
                yield return node;
                if(node.Children != null)
                    foreach (var n in node.Children) nodes.Push(n);
            }
        }

        public static string GetRelativePath(this BusinessEntityNode node)
        {
            List<string> listofNames = new List<string>();

            while(node.Parent != null)
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
