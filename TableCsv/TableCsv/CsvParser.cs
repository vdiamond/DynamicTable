
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;

namespace TableCsv
{

/// <summary>
/// Determines how empty lines are interpreted when reading CSV files.
/// These values do not affect empty lines that occur within quoted fields
/// or empty lines that appear at the end of the input file.
/// </summary>
	public enum EmptyLineBehavior
	{
		/// <summary>
		/// Empty lines are interpreted as a line with zero columns.
		/// </summary>
		NoColumns,
		/// <summary>
		/// Empty lines are interpreted as a line with a single empty column.
		/// </summary>
		EmptyColumn,
		/// <summary>
		/// Empty lines are skipped over as though they did not exist.
		/// </summary>
		Ignore,
		/// <summary>
		/// An empty line is interpreted as the end of the input file.
		/// </summary>
		EndOfFile,
	}

/// <summary>
/// Common base class for CSV reader and writer classes.
/// </summary>
	public abstract class CsvIO 
	{
		/// <summary>
		/// These are special characters in CSV files. If a column contains any
		/// of these characters, the entire column is wrapped in double quotes.
		/// </summary>
		protected char[] SpecialChars = new char[] { ',', '"', '\r', '\n' };
		// Indexes into SpecialChars for characters with specific meaning
		private const int DelimiterIndex = 0;
		private const int QuoteIndex = 1;
		/// <summary>
		/// Gets/sets the character used for column delimiters.
		/// </summary>
		public char Delimiter 
		{
			get { return SpecialChars [DelimiterIndex]; }
			set { SpecialChars [DelimiterIndex] = value; }
		}
		/// <summary>
		/// Gets/sets the character used for column quotes.
		/// </summary>
		public char Quote 
		{
			get { return SpecialChars [QuoteIndex]; }
			set { SpecialChars [QuoteIndex] = value; }
		}
	}
	public class CsvReader : CsvIO
	{
		public CsvReader (TextReader reader, EmptyLineBehavior emptyLineBehavior = EmptyLineBehavior.NoColumns)
		{
			_reader = reader;
			_emptyLineBehavior = emptyLineBehavior;
			_currLine = "";
		}
		public static List<List<string>> ReadAll(TextReader reader) 
		{
			var cfr = new CsvReader(reader);
			List<List<string>> dataGrid = new List<List<string>>();
			if(cfr.ReadAll(dataGrid)) return dataGrid;
			return null;
		}
		public bool ReadAll(List<List<string>> dataGrid) 
		{
			// Verify required argument
			if (dataGrid == null) {	throw new ArgumentNullException ("dataGrid"); }
			List<string> row = new List<string>();
			while (this.ReadRow(row)) {
				dataGrid.Add(new List<string>(row));
			}
			return true;
		}
		public bool ReadRow(List<string> columns)
		{
			// Verify required argument
			if (columns == null) { throw new ArgumentNullException("columns"); }

		ReadNextLine:
			// Read next line from the file
			_currLine = _reader.ReadLine();
			_currPos = 0;
			// Test for end of file
			if (_currLine == null) { return false; }
			// Test for empty line
			if (_currLine.Length == 0) {
				switch (_emptyLineBehavior) {
					case EmptyLineBehavior.NoColumns:
						columns.Clear();
						return true;
					case EmptyLineBehavior.Ignore:
						goto ReadNextLine;
					case EmptyLineBehavior.EndOfFile:
						return false;
				}
			}
			// Parse line
			string column;
			int numColumns = 0;
			while (true) {
				// Read next column
				if (_currPos < _currLine.Length && _currLine[_currPos] == Quote)
				{
					column = _readQuotedColumn();
				}
				else
				{
					column = _readUnquotedColumn();
				}
				// Add column to list
				if (numColumns < columns.Count)
				{
					columns[numColumns] = column;
				}
				else
				{
					columns.Add(column);
				}
				numColumns++;
				// Break if we reached the end of the line
				if (_currLine == null || _currPos == _currLine.Length) { break; }
				// Otherwise skip delimiter
				Debug.Assert(_currLine[_currPos] == Delimiter);
				_currPos++;
			}
			// Remove any unused columns from collection
			if (numColumns < columns.Count)
			{ 
				columns.RemoveRange(numColumns, columns.Count - numColumns);
			}
			// Indicate success
			return true;
		}
        #region private
        private string _readQuotedColumn()
		{
			// Skip opening quote character
			Debug.Assert (_currPos < _currLine.Length && _currLine [_currPos] == Quote);
			_currPos++;
		
			// Parse column
			StringBuilder builder = new StringBuilder ();
			while (true) {
				while (_currPos == _currLine.Length) {
					// End of line so attempt to read the next line
					_currLine = _reader.ReadLine ();
					_currPos = 0;
					// Done if we reached the end of the file
					if (_currLine == null)
						return builder.ToString ();
					// Otherwise, treat as a multi-line field
					builder.Append (Environment.NewLine);
				}
				// Test for quote character
				if (_currLine [_currPos] == Quote) {
					// If two quotes, skip first and treat second as literal
					int nextPos = (_currPos + 1);
					if (nextPos < _currLine.Length && _currLine [nextPos] == Quote)
						_currPos++;
					else
						break;  // Single quote ends quoted sequence
				}
				// Add current character to the column
				builder.Append (_currLine [_currPos++]);
			}
			if (_currPos < _currLine.Length) {
				// Consume closing quote
				Debug.Assert (_currLine [_currPos] == Quote);
				_currPos++;
				// Append any additional characters appearing before next delimiter
				builder.Append (_readUnquotedColumn());
			}
			// Return column value
			return builder.ToString ();
		}
		private string _readUnquotedColumn()
		{
			int startPos = _currPos;
			_currPos = _currLine.IndexOf (Delimiter, _currPos);
			if (_currPos == -1)
				_currPos = _currLine.Length;
			if (_currPos > startPos)
				return _currLine.Substring (startPos, _currPos - startPos);
			return String.Empty;
		}
        private TextReader _reader;
        private string _currLine;
        private int _currPos;
        private EmptyLineBehavior _emptyLineBehavior;
        #endregion
    }
    public class CsvWriter : CsvIO
	{
		public CsvWriter (TextWriter writer)
		{
			_writer = writer;
		}
		public static void WriteAll(List<List<string>> dataGrid, TextWriter writer) 
		{
			var cfw = new CsvWriter(writer);
			foreach(var row in dataGrid) 
			{
				cfw.WriteRow(row);
			}
		}
		public void WriteAll(List<List<string>> dataGrid) {
			foreach (List<string> row in dataGrid) 
			{
				this.WriteRow (row);
			}
		}
		public void WriteRow (List<string> columns)
		{
			// Verify required argument
			if (columns == null)
			{
				throw new ArgumentNullException("columns");
			}
			// Ensure we're using current quote character
			if (_oneQuote == null || _oneQuote[0] != Quote) 
			{
				_oneQuote = String.Format("{0}", Quote);
				_twoQuotes = String.Format("{0}{0}", Quote);
				_quotedFormat = String.Format("{0}{{0}}{0}", Quote);
			}
			// Write each column
			for (int i = 0; i < columns.Count; i++) 
			{
				// Add delimiter if this isn't the first column
				if (i > 0)
				{
					_writer.Write(Delimiter);
				}
				// Write this column
				if (columns[i].IndexOfAny(SpecialChars) == -1)
				{
					_writer.Write(columns[i]);
				}
				else
				{
					_writer.Write(_quotedFormat, columns[i].Replace(_oneQuote, _twoQuotes));
				}
			}
			_writer.Write ("\r\n");
		}
		public void Flush()
		{
			_writer.Flush();
		}
        #region private
        private TextWriter _writer;
        private string _oneQuote = null;
        private string _twoQuotes = null;
        private string _quotedFormat = null;
        #endregion
    }

}

