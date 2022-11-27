variable "curr_vpc" {
  description = "VPC to use"
  type        = string
}

variable "private_subnets" {
  description = "Private subnets"
  type        = list(string)
}
