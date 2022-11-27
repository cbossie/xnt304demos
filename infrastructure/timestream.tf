# TimeStream DB
resource "aws_timestreamwrite_database" "reinvent_timestream" {
  database_name = "reinvent"
}

resource "aws_timestreamwrite_table" "reinvent_table" {
  database_name = aws_timestreamwrite_database.reinvent_timestream.database_name
  table_name    = "reinventTable"
}