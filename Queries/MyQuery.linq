<Query Kind="Statements">
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

"Success2".Dump();

string myComputerPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

Process.Start("explorer", myComputerPath);