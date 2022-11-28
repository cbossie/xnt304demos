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



#DynamoDB Table - Character
resource "aws_dynamodb_table" "characters" {
  name         = "Characters"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "Name"
  stream_enabled   = true
  stream_view_type = "NEW_AND_OLD_IMAGES"
  attribute {
    name = "Name"
    type = "S"
  }
  replica {
    region_name = "eu-west-1"
  }
}

#Global Table
resource "aws_dynamodb_table_replica" "characters_euwest" {
  provider = aws.euwest
  global_table_arn = aws_dynamodb_table.characters.arn
}
