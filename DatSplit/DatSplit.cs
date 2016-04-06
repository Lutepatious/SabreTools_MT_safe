﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using SabreTools.Helper;

namespace DatSplit
{
	class DatSplit
	{
		private static string extA;
		private static string extB;
		private static string filename;

		public static void Main(string[] args)
		{
			Console.Title = "DatSplit " + Build.Version;

			// If we don't have arguments, show help
			if (args.Length == 0 && args.Length != 3)
			{
				Help();
				return;
			}

			// Set needed strings
			filename = args[0];
			extA = (args[1].StartsWith(".") ? args[1] : "." + args[1]);
			extB = (args[2].StartsWith(".") ? args[2] : "." + args[2]);

			// Take the filename, and load it as an XML document
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(File.ReadAllText(filename));
			}
			catch (XmlException)
			{
				doc.LoadXml(Converters.RomVaultToXML(File.ReadAllLines(filename)).ToString());
			}

			// We all start the same
			XmlNode node = doc.FirstChild;
			if (node != null && node.Name == "xml")
			{
				// Skip over everything that's not an element
				while (node.NodeType != XmlNodeType.Element)
				{
					node = node.NextSibling;
				}
			}

			XmlDocument outDocA = new XmlDocument();
			outDocA.AppendChild(outDocA.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, null));
			outDocA.AppendChild(outDocA.CreateDocumentType("datafile", "-//Logiqx//DTD ROM Management Datafile//EN", "http://www.logiqx.com/Dats/datafile.dtd", null));
			XmlNode outA = outDocA.CreateNode(XmlNodeType.Element, node.Name, "");

			XmlDocument outDocB = new XmlDocument();
			outDocB.AppendChild(outDocB.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, null));
			outDocB.AppendChild(outDocB.CreateDocumentType("datafile", "-//Logiqx//DTD ROM Management Datafile//EN", "http://www.logiqx.com/Dats/datafile.dtd", null));
			XmlNode outB = outDocB.CreateNode(XmlNodeType.Element, node.Name, "");

			// Once we find the main body, enter it
			if (node != null && node.Name == "datafile")
			{
				node = node.FirstChild;
			}

			// Now here's where it differs from import
			while (node != null)
			{
				// If we're at a game node, add the parent node but not all the internals
				if (node.NodeType == XmlNodeType.Element && (node.Name == "machine" || node.Name == "game"))
				{
					bool inA = false;
					bool inB = false;

					// Get the roms from the machine
					if (node.HasChildNodes)
					{
						// If this node has children, traverse the children
						foreach (XmlNode child in node.ChildNodes)
						{
							// If we find a rom or disk, add it
							if (child.NodeType == XmlNodeType.Element && (child.Name == "rom" || child.Name == "disk"))
							{
								// Take care of hex-sized files
								long size = -1;
								if (child.Attributes["size"] != null && child.Attributes["size"].Value.Contains("0x"))
								{
									size = Convert.ToInt64(child.Attributes["size"].Value, 16);
								}
								else if (child.Attributes["size"] != null)
								{
									size = Int64.Parse(child.Attributes["size"].Value);
								}

								if (child.Attributes["name"].Value.EndsWith(extA))
								{
									if (!inA)
									{
										//XmlNode temp = tempDoc.CreateNode(XmlNodeType.Element, node.Name, "");
										XmlNode temp = outDocA.ImportNode(node, false);
										outA.AppendChild(temp);
										outA = outA.LastChild;
										inA = true;
									}
									outA.AppendChild(outDocA.ImportNode(child, true));
								}
								else if (child.Attributes["name"].Value.EndsWith(extB))
								{
									if (!inB)
									{
										//XmlNode temp = tempDoc.CreateNode(XmlNodeType.Element, node.Name, "");
										XmlNode temp = outDocB.ImportNode(node, false);
										outB.AppendChild(temp);
										outB = outB.LastChild;
										inB = true;
									}
									outB.AppendChild(outDocB.ImportNode(child, true));
								}
								else
								{
									outA.AppendChild(outDocA.ImportNode(child, true));
									outB.AppendChild(outDocB.ImportNode(child, true));
								}
							}
						}

						// Set the output node to the right one for both
						if (inA)
						{
							outA = outA.ParentNode;
						}
						if (inB)
						{
							outB = outB.ParentNode;
						}
					}
				}
				else
				{
					XmlNode tempNode = outDocA.ImportNode(node, true);
					outA.AppendChild(tempNode);
					tempNode = outDocB.ImportNode(node, true);
					outB.AppendChild(tempNode);
				}
				node = node.NextSibling;
			}

			// Append the built nodes to the documents
			outDocA.AppendChild(outDocA.ImportNode(outA, true));
			string outPathA = Path.GetFileNameWithoutExtension(filename) + extA + Path.GetExtension(filename);
			File.WriteAllText(outPathA, Beautify(outDocA), Encoding.UTF8);

			outDocB.AppendChild(outDocB.ImportNode(outB, true));
			string outPathB = Path.GetFileNameWithoutExtension(filename) + extB + Path.GetExtension(filename);
			File.WriteAllText(outPathB, Beautify(outDocB), Encoding.UTF8);
		}

		public static void Help()
		{
			Console.WriteLine("DatSplit.exe <filename> <ext> <ext>");
		}

		// http://stackoverflow.com/questions/203528/what-is-the-simplest-way-to-get-indented-xml-with-line-breaks-from-xmldocument
		// http://www.timvw.be/2007/01/08/generating-utf-8-with-systemxmlxmlwriter/
		static public string Beautify(XmlDocument doc)
		{
			MemoryStream ms = new MemoryStream();
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Encoding = new UTF8Encoding(false),
				Indent = true,
				IndentChars = "\t",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace
			};
			
			using (XmlWriter writer = XmlWriter.Create(ms, settings))
			{
				doc.Save(writer);
			}
			return Encoding.UTF8.GetString(ms.ToArray());
		}
	}
}
