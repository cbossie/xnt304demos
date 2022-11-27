#Elasticache for Redis
resource "aws_elasticache_cluster" "reinvent_elasticache" {
  num_cache_nodes    = 1
  engine             = "redis"
  node_type          = "cache.t4g.small"
  cluster_id         = "reinvent-cluster"
  security_group_ids = [aws_security_group.default_security_group.id]
  apply_immediately  = true
  subnet_group_name  = aws_elasticache_subnet_group.reinvent_ec_group.name
}

resource "aws_elasticache_subnet_group" "reinvent_ec_group" {
  name       = "elasticache-subnetgroup"
  subnet_ids = var.private_subnets
}