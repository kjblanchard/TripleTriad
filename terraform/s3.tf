resource "aws_s3_bucket" "triple_triad_bucket" {
  bucket = "triple-triad-external-site"
  tags = {
    Name = "Triple Triad External Site Bucket"
  }
}
resource "aws_s3_bucket_ownership_controls" "s3_bucket_acl_ownership" {
  bucket = aws_s3_bucket.triple_triad_bucket.id
  rule {
    object_ownership = "ObjectWriter"
  }
}

resource "aws_iam_user" "triple_triad_user" {
  name = "triple-triad-uploader"
}

resource "aws_s3_bucket_acl" "b_acl" {
  bucket = aws_s3_bucket.triple_triad_bucket.id
  depends_on = [ aws_s3_bucket_ownership_controls.s3_bucket_acl_ownership ]
  acl    = "private"
}

resource "aws_iam_policy" "pipeline_access_policy" {
  name        = "triple_triad_s3_upload_policy"
  description = "Policy for adding items to the triple triad bucket."

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [{
      Effect = "Allow",
      Action = ["s3:PutObject", "s3:ListBucket"],
      Resource = [
        aws_s3_bucket.triple_triad_bucket.arn,
        "${aws_s3_bucket.triple_triad_bucket.arn}/*",
      ],
    }],
  })
}

resource "aws_iam_user_policy_attachment" "triple_triad_attachment" {
  policy_arn = aws_iam_policy.pipeline_access_policy.arn
  user       = aws_iam_user.triple_triad_user.name
}
