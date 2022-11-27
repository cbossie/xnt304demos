#MemoryDB
resource "aws_memorydb_subnet_group" "reinventsubnetgroup" {
  name       = "reinvent-subnet-group"
  subnet_ids = var.private_subnets
}

resource "aws_memorydb_cluster" "reinvent_memdb" {
  acl_name                 = "open-access"
  name                     = "reinvent-cluster"
  node_type                = "db.t4g.small"
  num_shards               = 3
  security_group_ids       = [aws_security_group.default_security_group.id]
  snapshot_retention_limit = 7
  subnet_group_name        = aws_memorydb_subnet_group.reinventsubnetgroup.name
}