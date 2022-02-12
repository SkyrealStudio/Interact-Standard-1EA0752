namespace Skyreal.Universal.Tests
{
    using Static;
    class MainClass
    {
        private static void Main(string[] args)
        {
            Queue<TextScriptReader.TextScriptMethod> tx = TextScriptReader.GetMethods("C:\\Users\\thewa\\Documents\\Codes\\Interact-Standard-1EA0752\\instances\\CSharp\\Test\\test.txt");
            while (tx.Count > 0)
            {
                TextScriptReader.TextScriptMethod method = tx.Dequeue();
                Console.WriteLine("Method Name: " + method.MethodName);
                foreach (string str in method.Parameters)
                    Console.WriteLine("-: " + str);
            }
        }
    }
}
