output "security_group" {
  value = aws_security_group.default_security_group.name
}


output "memorydb_endpoint" {
  description = "The endpoint of the cluster"
  value       = aws_memorydb_cluster.reinvent_memdb.cluster_endpoint
}