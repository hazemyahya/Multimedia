using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
        string decoded;
        private void label1_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            var tree = new HuffTree();
            string encoded = tree.Encode(text);

            tree.Reset();
            textBox2.Text = encoded;
            decoded = tree.Decode(textBox2.Text);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            textBox3.Text = decoded;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 Form1 = new Form1();
            Form1.Show();
            this.Hide();
        }
    }
    public class Node
    {
        public char? Symbol { get; set; }
        public int Weight { get; set; }
        public int Number { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Parent { get; set; }
        public bool IsNYT { get; set; }

        public Node()
        { }

        public Node(Node parent)
        {
            Parent = parent;
        }

        public Node(Node parent, char symbol)
        {
            Parent = parent;
            Symbol = symbol;
            Weight = 1;
        }

        public Node FindOrDefault(char symbol)
        {
            if (Symbol == symbol)
                return this;

            Node result = Left?.FindOrDefault(symbol);
            if (result != null)
                return result;

            return Right?.FindOrDefault(symbol);
        }

        public string GetCode(Node searched)
        {
            return GetCode(searched, String.Empty);
        }

        private string GetCode(Node searched, string code)
        {
            if (Symbol == searched.Symbol)
                return code;

            if (Left == null && Right == null)
                return null;

            string result = Left.GetCode(searched, code + "0");
            if (result != null)
                return result;

            return Right.GetCode(searched, code + "1");
        }

        public string GetNYTCode(string code)
        {
            if (IsNYT)
                return code;

            if (Left == null && Right == null)
                return null;

            string result = Left.GetNYTCode(code + "0");
            if (result != null)
                return result;

            return Right.GetNYTCode(code + "1");
        }

        public bool IsLeftSon(Node son)
        {
            return Left == son;
        }

        public bool IsRightSon(Node son)
        {
            return Right == son;
        }

        public bool IsLeaf() // symbol == null ??
        {
            return Left == null && Right == null;
        }
    }

    public class HuffTree
    {
        public Node Root { get; private set; }

        private Node _nyt; // "not yet transfered"
        private Node[] _nodes;
        private int _nextNum;

        public HuffTree()
        {
            Reset();
        }

        public void Reset()
        {
            Root = new Node { Number = 512 };
            _nyt = Root;
            _nodes = new Node[513];
            _nodes[Root.Number] = Root;
            _nextNum = 511;
        }

        public string Encode(string text)
        {
            var result = new StringBuilder();

            foreach (var c in text)
                result.Append(Encode(c));

            return result.ToString();
        }

        public string Encode(char symbol)
        {
            Node node = Root.FindOrDefault(symbol);

            string code;

            if (node != null)
            {
                code = Root.GetCode(node);
                node.Weight++;
            }
            else
            {
                code = Root.GetNYTCode(string.Empty);
                code += symbol;
                node = AddToNYT(symbol);
            }

            UpdateAll(node.Parent);

            return code;
        }

        public string Decode(string code)
        {
            var result = new StringBuilder();

            int index = 0;
            while (index < code.Length)
            {
                Node node;

                char? symbol = ReadChar(index, code, out int count);
                index += count;

                if (symbol == null)
                {
                    symbol = code[index - 1];
                    node = AddToNYT(symbol.Value);
                }
                else
                {
                    node = Root.FindOrDefault(symbol.Value);
                    node.Weight++;
                }

                UpdateAll(node.Parent);

                result.Append(symbol.Value);
            }

            return result.ToString();
        }

        private char? ReadChar(int index, string code, out int count)
        {
            Node current = Root;
            count = 0;

            while (true)
            {
                count++;

                if (current == _nyt)
                    return null;

                if (current.IsLeaf())
                {
                    count--;
                    return current.Symbol;
                }

                char bit = code[index++];

                if (bit == '0')
                    current = current.Left;
                else if (bit == '1')
                    current = current.Right;
            }
        }

        private Node AddToNYT(char symbol)
        {
            var node = new Node(_nyt, symbol)
            {
                Number = _nextNum
            };
            _nyt.Right = node;
            _nodes[_nextNum--] = node;

            var nyt = new Node(_nyt)
            {
                Number = _nextNum,
                IsNYT = true
            };
            _nyt.IsNYT = false;
            _nyt.Left = nyt;
            _nodes[_nextNum--] = nyt;

            _nyt = nyt;

            return node;
        }

        private void UpdateAll(Node node)
        {
            while (node != null)
            {
                Update(node);
                node = node.Parent;
            }
        }

        private void Update(Node node)
        {
            Node toReplace = NodeToReplace(node.Number, node.Weight);

            if (toReplace != null && node.Parent != toReplace)
                Replace(node, toReplace);

            node.Weight++;
        }

        private Node NodeToReplace(int startIndex, int weight)
        {
            startIndex++;
            Node found = null;

            for (int i = startIndex; i < _nodes.Length; i++)
                if (_nodes[i].Weight == weight)
                    found = _nodes[i];

            return found;
        }

        private void Replace(Node a, Node b)
        {
            ReplaceNumbers(a, b);
            ReplaceSons(a, b);
        }

        private void ReplaceNumbers(Node a, Node b)
        {
            Node temp = _nodes[a.Number];
            _nodes[a.Number] = _nodes[b.Number];
            _nodes[b.Number] = temp;

            int tempNum = a.Number;
            a.Number = b.Number;
            b.Number = tempNum;
        }

        private void ReplaceSons(Node a, Node b)
        {
            bool bIsLeftSon = b.Parent.IsLeftSon(b);

            if (a.Parent.IsLeftSon(a))
                a.Parent.Left = b;
            else
                a.Parent.Right = b;

            Node temp = b.Parent;
            b.Parent = a.Parent;
            a.Parent = temp;

            if (bIsLeftSon)
                temp.Left = a;
            else
                temp.Right = a;
        }
    }
}
