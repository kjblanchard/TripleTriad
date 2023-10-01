package main

import (
	"fmt"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/s3/s3manager"
	"os"
	"path/filepath"
)

// var (
// 	filename = "index.html"
// 	myBucket = "triple-triad-external-site"
// 	myString = "index.html"
// )

func main() {

	// folderPath := os.Getenv("FOLDER_PATH")
	folderPath := "files"
	// myBucket := os.Getenv("BUCKET_NAME")
	myBucket := "triple-triad-external-site"

	sess := session.Must(session.NewSession())
	uploader := s3manager.NewUploader(sess)

	err := filepath.Walk(folderPath, func(path string, info os.FileInfo, err error) error {
		if err != nil {
			return err
		}

		if info.IsDir() {
			return nil
		}

		s3File, err := filepath.Rel(folderPath, path)
		if err != nil {
			return err
		}

		f, err := os.Open(path)
		if err != nil {
			return err
		}

		result, err := uploader.Upload(&s3manager.UploadInput{
			Bucket: aws.String(myBucket),
			Key:    &s3File,
			Body:   f,
		})
		if err != nil {
			fmt.Fprintf(os.Stderr, "failed to upload file, %v", err)
			os.Exit(1)
		}

		fmt.Printf("file uploaded to, %s\n", result.Location)
		return nil
	})

	if err != nil {
		fmt.Fprintf(os.Stderr, "Failed to walk the directory %s, %v", folderPath, err)
	}

}
