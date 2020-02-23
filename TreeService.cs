using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    /// <summary>
    /// 树的遍历
    /// </summary>
    class TreeService
    {
        public Tree CreatFakeTree()
        {
            Tree tree = new Tree() { Value = "A" };
            tree.Left = new Tree()
            {
                Value = "B",
                Left = new Tree() { Value = "D", Left = new Tree() { Value = "G" } },
                Right = new Tree() { Value = "E", Right = new Tree() { Value = "H" } }
            };
            tree.Right = new Tree() { Value = "C", Right = new Tree() { Value = "F" } };

            return tree;
        }

        #region 先序遍历

        //先序遍历还是很好理解的，一次遍历根节点，左子树，右子数
        //递归实现
        //打印出，ABDGEHCF
        public  void PreOrder(Tree tree)
        {
            if (tree == null)
                return;

            System.Console.WriteLine(tree.Value);
            PreOrder(tree.Left);
            PreOrder(tree.Right);
        }
        //非递归实现
        public  void PreOrderNoRecursion(Tree tree)
        {
            if (tree == null)
                return;

            System.Collections.Generic.Stack<Tree> stack = new System.Collections.Generic.Stack<Tree>();
            Tree node = tree;

            while (node != null || stack.Any())
            {
                if (node != null)
                {
                    stack.Push(node);
                    System.Console.WriteLine(node.Value);
                    node = node.Left;
                }
                else
                {
                    var item = stack.Pop();
                    node = item.Right;
                }
            }
        }
        #endregion

        #region 中序遍历
        //递归实现
        //GDBEHACF
         public  void InOrder(Tree tree)
        {
            if(tree == null)
                return;

            InOrder(tree.Left);
            System.Console.WriteLine(tree.Value);
            InOrder(tree.Right);
        } 

        //非递归实现
        public  void InOrderNoRecursion(Tree tree)
        {
            if (tree == null)
                return;

            System.Collections.Generic.Stack<Tree> stack = new System.Collections.Generic.Stack<Tree>();
            Tree node = tree;

            while (node != null || stack.Any())
            {
                if (node != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }
                else
                {
                    var item = stack.Pop();
                    System.Console.WriteLine(item.Value);

                    node = item.Right;
                }
            }
        }
        #endregion
        #region 后序遍历
        //GDHEBFCA
        //递归实现
        public  void PostOrder(Tree tree)
        {
            if (tree == null)
                return;

            PostOrder(tree.Left);
            PostOrder(tree.Right);
            System.Console.WriteLine(tree.Value);
        }

        #endregion
        #region 层序遍历，按照层次从左向右
        //ABCDEFGH
        public  void LevelOrder(Tree tree)
        {
            if (tree == null)
                return;

            Queue<Tree> queue = new Queue<Tree>();
            queue.Enqueue(tree);

            while (queue.Any())
            {
                var item = queue.Dequeue();
                System.Console.Write(item.Value);

                if (item.Left != null)
                {
                    queue.Enqueue(item.Left);
                }

                if (item.Right != null)
                {
                    queue.Enqueue(item.Right);
                }
            }
        }
        #endregion
    }
    

    public class Tree {
        public string Value;
        public Tree Left;
        public Tree Right;
    }

    
}
