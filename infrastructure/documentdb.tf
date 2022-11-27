#Document DB
resource "aws_docdb_subnet_group" "docdb_subnet_group" {
  name       = "docdb-subnetgroup"
  subnet_ids = var.private_subnets
}

resource "aws_docdb_cluster" "docdb_cluster" {
  cluster_identifier     = "reinvent-docdb-cluster"
  master_username        = "reinvent"
  master_password        = "!winxnet1"
  vpc_security_group_ids = [aws_security_group.default_security_group.id]
  apply_immediately      = true
  db_subnet_group_name   = aws_docdb_subnet_group.docdb_subnet_group.name
}

resource "aws_docdb_cluster_instance" "docdb_instances" {
  cluster_identifier = aws_docdb_cluster.docdb_cluster.id
  count              = 2
  instance_class     = "db.t3.medium"
  identifier         = "${aws_docdb_cluster.docdb_cluster.cluster_identifier}-${count.index}"
}