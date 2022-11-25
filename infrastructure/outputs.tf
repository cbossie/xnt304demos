output "security_group" {
  value = aws_security_group.default_security_group.name
}


output "memorydb_endpoint" {
  description = "The endpoint of the cluster"
  value       = aws_memorydb_cluster.reinvent_memdb.cluster_endpoint
}

output "dax_endpoint" {
  description = "The DAX cluster endpoint"
  value       = aws_dax_cluster.orders_dax_cluster.configuration_endpoint
}


output "dax_address" {
  description = "The DAX cluster endpoint URL"
  value       = aws_dax_cluster.orders_dax_cluster.cluster_address
}

output "dax_port" {
  description = "The DAX cluster endpoint URL"
  value       = aws_dax_cluster.orders_dax_cluster.port
}













