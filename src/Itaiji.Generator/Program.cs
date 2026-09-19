// See https://aka.ms/new-console-template for more information
using Itaiji.Generator;
using System.Text;
Console.OutputEncoding = System.Text.Encoding.UTF8;


if (args is ["--binary-only"])
{
    BinaryIvdSequences.Execute();
}
else
{
    IvdSequences.Execute();
    BinaryIvdSequences.Execute();
    StandardizedVariants.Execute();
}
