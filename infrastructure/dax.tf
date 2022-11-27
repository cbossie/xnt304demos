#Role for Dax
resource "aws_iam_role" "dax_table_role" {
  name = "dax_role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Sid    = "Assume"
        Principal = {
          Service = "dax.amazonaws.com"
        }
      },
    ]
  })
}

#Policy for table
resource "aws_iam_role_policy" "dax_table_policy" {
  name = "dax-dynamodb-policy"
  role = aws_iam_role.dax_table_role.name
  policy = jsonencode(
    {
      Version : "2012-10-17",
      Statement : [
        {
          Action : [
            "dynamodb:DescribeTable",
            "dynamodb:PutItem",
            "dynamodb:GetItem",
            "dynamodb:UpdateItem",
            "dynamodb:DeleteItem",
            "dynamodb:Query",
            "dynamodb:Scan",
            "dynamodb:BatchGetItem",
            "dynamodb:BatchWriteItem",
            "dynamodb:ConditionCheckItem"
          ],
          Effect : "Allow",
          Resource : [
            "${aws_dynamodb_table.orders.arn}"
          ]
        }
      ]
  })
}

#Dax Cluster and Related
resource "aws_dax_subnet_group" "orders_dax" {
  name       = "orders-dax"
  subnet_ids = var.private_subnets
}


resource "aws_dax_cluster" "orders_dax_cluster" {
  cluster_name       = "order-dax-cluster"
  iam_role_arn       = aws_iam_role.dax_table_role.arn
  node_type          = "dax.t3.medium"
  replication_factor = 4
  security_group_ids = [aws_security_group.default_security_group.id]
  subnet_group_name  = aws_dax_subnet_group.orders_dax.name
}