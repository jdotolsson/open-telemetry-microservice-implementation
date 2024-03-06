package database

import (
	"context"
	"database/sql"
	"database/sql/driver"
	types "example/order-service/internal/models"
	"fmt"
	"log"
	"os"

	"go.opentelemetry.io/otel"
	semconv "go.opentelemetry.io/otel/semconv/v1.18.0"
	"go.opentelemetry.io/otel/trace"

	"github.com/XSAM/otelsql"
	"github.com/go-sql-driver/mysql"
)

var (
	tracer = otel.Tracer("order")
)

func connectToDb() *sql.DB {
	// Capture connection properties.
	cfg := mysql.Config{
		User:   os.Getenv("DB_USER"),
		Passwd: os.Getenv("DB_PASS"),
		Net:    "tcp",
		Addr:   os.Getenv("DB_ENDPOINT"),
		DBName: os.Getenv("DB_DATABASE"),
	}
	// Get a database handle.
	db, err := otelsql.Open("mysql", cfg.FormatDSN(),
		otelsql.WithAttributes(
			semconv.DBSystemMySQL,
		),
		otelsql.WithSpanOptions(otelsql.SpanOptions{
			Ping:                 false,
			RowsNext:             false,
			DisableErrSkip:       true,
			DisableQuery:         false,
			OmitConnResetSession: true,
			OmitConnPrepare:      true,
			OmitConnQuery:        true,
			OmitRows:             false,
			OmitConnectorConnect: true,
			SpanFilter: func(ctx context.Context, method otelsql.Method, query string, args []driver.NamedValue) bool {
				span := trace.SpanFromContext(ctx)
				return span.SpanContext().IsValid()
			},
		}),
	)

	if err != nil {
		log.Fatal(err)
	}

	err = otelsql.RegisterDBStatsMetrics(db, otelsql.WithAttributes(
		semconv.DBSystemMySQL,
	))
	if err != nil {
		log.Fatal(err)
	}

	pingErr := db.Ping()
	if pingErr != nil {
		log.Fatal(pingErr)
	}
	return db
}

func AddOrder(order types.Order, ctx context.Context) (int64, error) {
	db := connectToDb()

	ctx, span := tracer.Start(ctx, "order.db")
	defer span.End()

	id, err := innerAddOrder(order, db, ctx)
	if err != nil {
		span.RecordError(err)
		return 0, err
	}
	defer db.Close()

	return id, nil
}

func innerAddOrder(order types.Order, db *sql.DB, ctx context.Context) (int64, error) {
	result, err := db.ExecContext(ctx, "INSERT INTO `order` (`id`) VALUES (?)", order.ID)
	if err != nil {
		fmt.Println(err)
		return 0, fmt.Errorf("addOrder: %v", err)
	}
	id, err := result.LastInsertId()
	if err != nil {
		fmt.Println(err)
		return 0, fmt.Errorf("addOrder: %v", err)
	}
	return id, nil
}
