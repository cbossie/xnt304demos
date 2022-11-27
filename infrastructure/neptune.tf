#Neptune
resource "aws_neptune_subnet_group" "neptune_subnet_group" {
  name       = "neptune-subnetgroup"
  subnet_ids = var.private_subnets
}

resource "aws_neptune_cluster" "neptune_cluster" {
  cluster_identifier                   = "reinvent-cluster"
  engine                               = "neptune"
  deletion_protection                  = false
  iam_database_authentication_enabled  = true
  apply_immediately                    = true
  neptune_subnet_group_name            = aws_neptune_subnet_group.neptune_subnet_group.name
  vpc_security_group_ids               = [aws_security_group.default_security_group.id]
  neptune_cluster_parameter_group_name = aws_neptune_cluster_parameter_group.neptune_pg.name
}

resource "aws_neptune_cluster_instance" "neptune_instances" {
  count                        = 2
  cluster_identifier           = aws_neptune_cluster.neptune_cluster.id
  engine                       = "neptune"
  instance_class               = "db.t3.medium"
  apply_immediately            = true
  identifier                   = "${aws_neptune_cluster.neptune_cluster.cluster_identifier}-${count.index}"
  neptune_parameter_group_name = "neptune12"
  neptune_subnet_group_name    = aws_neptune_subnet_group.neptune_subnet_group.name
}

resource "aws_neptune_cluster_parameter_group" "neptune_pg" {
  family      = "neptune1.2"
  name        = "neptunepg"
  description = "Neptune cluster parameter group"
}