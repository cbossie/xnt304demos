data "aws_vpc" "curr_vpc" {
  id = var.curr_vpc
}

data "aws_security_group" "default_sg" {
  name   = "default"
  vpc_id = var.curr_vpc
}