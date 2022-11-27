# Quantum Ledger Database
resource "aws_qldb_ledger" "qldb" {
  name                = "reinvent-qldb"
  deletion_protection = false
  permissions_mode    = "ALLOW_ALL"
}