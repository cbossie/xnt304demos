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
    from_port   = 0
    to_port     = 0
    protocol    = "tcp"
    cidr_blocks = data.aws_vpc.curr_vpc.cidr_block_associations[*].cidr_block

  }

  egress {
    from_port        = 0
    to_port          = 0
    protocol         = "-1"
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
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