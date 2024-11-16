using System;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // For .xlsx files
using NPOI.HSSF.UserModel; // For .xls files

Excel excel_setup = new Excel();
TextEditor mission = new TextEditor();
Queue<Queue_Node> info = new Queue<Queue_Node>();
Queue_Node queue_Node = new Queue_Node();
bool Sheet_status = Excel.Read_Cabinet_Sheet(info);
if (Sheet_status)
{
    Console.WriteLine($"Queue Size is {info.Count}");
    int counter = info.Count;
    for (int i = 0; i < counter; i++)
    {
        queue_Node = info.Dequeue();
        if (queue_Node.Cabinet_Status == "Accepted")
        {
            Console.WriteLine($"{queue_Node.Cabinet_Family_Name} | {queue_Node.Cabinet_Type} | {queue_Node.Code_1} | {queue_Node.Code_2}");
            mission = new TextEditor(queue_Node.Cabinet_Family_Name, queue_Node.Cabinet_Type, queue_Node.Code_1, queue_Node.Code_2);
            mission.CreateCommission();
            Console.WriteLine(".............................................................................................................");
        }
    }
}
public class Queue_Node
{
    public string? Cabinet_Status { get; set; }
    public string? Cabinet_Family_Name { get; set; }
    public string? Cabinet_Type { get; set; }
    public string? Code_1 { get; set; }
    public string? Code_2 { get; set; }
}