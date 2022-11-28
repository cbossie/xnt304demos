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

# Provider for our global table
provider "aws" {
  region  = "eu-west-1"
  profile = "reinvent"
  alias = "euwest"
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
  type              = "ingress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  security_group_id = aws_security_group.default_security_group.id
  cidr_blocks       = data.aws_vpc.curr_vpc.cidr_block_associations[*].cidr_block
  depends_on        = [aws_security_group.default_security_group]
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

















