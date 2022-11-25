terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
  backend "s3" {
    bucket  = "cb-reinvent-tf"
    key     = "purposebuilt"
    region  = "us-east-1"
    profile = "reinvent"
  }
}

# Configure the AWS Provider
provider "aws" {
  region  = "us-east-1"
  profile = "reinvent"
  default_tags {
    tags = {
      Environment = "Prod"
      System      = "ReInvent 2022"
    }
  }
}


#VPC and Networking
resource "aws_security_group" "default_security_group" {
  name        = "allow_from_self"
  description = "Allow from default security group"
  vpc_id      = var.curr_vpc

  ingress {
    from_port       = 0
    to_port         = 0
    protocol        = "-1"
    security_groups = [data.aws_security_group.default_sg.id]
  }

  egress {
    from_port        = 0
    to_port          = 0
    protocol         = "-1"
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
}

resource "aws_security_group_rule" "allow_from_local_cidr" {
  type                     = "ingress"
  from_port                = 0
  to_port                  = 0
  protocol                 = "-1"
  security_group_id        = aws_security_group.default_security_group.id
  cidr_blocks = data.aws_vpc.curr_vpc.cidr_block_associations[*].cidr_block
  depends_on               = [aws_security_group.default_security_group]
}

resource "aws_security_group_rule" "allow_from_self" {
  type                     = "ingress"
  from_port                = 0
  to_port                  = 0
  protocol                 = "-1"
  security_group_id        = aws_security_group.default_security_group.id
  source_security_group_id = aws_security_group.default_security_group.id
  depends_on               = [aws_security_group.default_security_group]
}



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


#DynamoDB Table - Order
resource "aws_dynamodb_table" "orders" {
  name         = "Orders"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "Id"

  attribute {
    name = "Id"
    type = "N"
  }

  attribute {
    name = "CustomerId"
    type = "S"
  }

  attribute {
    name = "Year"
    type = "N"
  }

  attribute {
    name = "Total"
    type = "N"
  }

  global_secondary_index {
    name               = "CustomerIdYear"
    hash_key           = "CustomerId"
    range_key          = "Year"
    projection_type    = "INCLUDE"
    non_key_attributes = ["Id"]
  }

  global_secondary_index {
    name               = "TotalIndex"
    hash_key           = "Total"
    projection_type    = "INCLUDE"
    non_key_attributes = ["Id", "CustomerId"]
  }

}

#Dax

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



#DynamoDB Table - Character
resource "aws_dynamodb_table" "characters" {
  name         = "Characters"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "Name"

  attribute {
    name = "Name"
    type = "S"
  }
}



