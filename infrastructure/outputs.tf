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

output "qldb_id" {
  description = "The QLDB database created for this application"
  value       = aws_qldb_ledger.qldb.id
}

output "timestream_id" {
  description = "Timestream Database"
  value       = aws_timestreamwrite_database.reinvent_timestream.id
}

output "docdb_endpoint" {
  description = "Document DB endpoint"
  value       = aws_docdb_cluster.docdb_cluster.endpoint
}

output "neptune_endpoint" {
  description = "Neptune Endpoint"
  value       = aws_neptune_cluster.neptune_cluster.endpoint
}

output "elasticache_endpoints" {
  description = "Elassticache cluster endpoint"
  value       = aws_elasticache_cluster.reinvent_elasticache.cache_nodes
}



