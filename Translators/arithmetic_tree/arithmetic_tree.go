package main

import (
	"fmt"
	"strconv"
	"strings"
)

type Node struct {
	Value string
	Left  *Node
	Right *Node
}

func NewNode(value string) *Node {
	return &Node{Value: value}
}

func (n *Node) IsOperator() bool {
	return n.Value == "+" || n.Value == "-" || n.Value == "*" || n.Value == "/"
}

func (n *Node) Evaluate() (float64, error) {
	if !n.IsOperator() {
		return strconv.ParseFloat(n.Value, 64)
	}

	left, err := n.Left.Evaluate()
	if err != nil {
		return 0, err
	}

	right, err := n.Right.Evaluate()
	if err != nil {
		return 0, err
	}

	switch n.Value {
	case "+":
		return left + right, nil
	case "-":
		return left - right, nil
	case "*":
		return left * right, nil
	case "/":
		if right == 0 {
			return 0, fmt.Errorf("division by zero")
		}
		return left / right, nil
	default:
		return 0, fmt.Errorf("unknown operator: %s", n.Value)
	}
}

func (n *Node) String() string {
	if !n.IsOperator() {
		return n.Value
	}
	return fmt.Sprintf("(%s %s %s)", n.Left.String(), n.Value, n.Right.String())
}

func ParseExpression(expr string) (*Node, error) {
	expr = strings.ReplaceAll(expr, " ", "")

	lowestPrec := -1
	lowestPos := -1
	parenCount := 0

	for i := len(expr) - 1; i >= 0; i-- {
		char := string(expr[i])
		if char == ")" {
			parenCount++
		} else if char == "(" {
			parenCount--
		} else if parenCount == 0 && (char == "+" || char == "-") {
			lowestPos = i
			lowestPrec = 1
			break
		} else if parenCount == 0 && (char == "*" || char == "/") && lowestPrec < 2 {
			lowestPos = i
			lowestPrec = 2
		}
	}

	if lowestPos == -1 {
		if expr[0] == '(' && expr[len(expr)-1] == ')' {
			return ParseExpression(expr[1 : len(expr)-1])
		}
		return NewNode(expr), nil
	}

	node := NewNode(string(expr[lowestPos]))

	left, err := ParseExpression(expr[:lowestPos])
	if err != nil {
		return nil, err
	}

	right, err := ParseExpression(expr[lowestPos+1:])
	if err != nil {
		return nil, err
	}

	node.Left = left
	node.Right = right

	return node, nil
}

func main() {
	expressions := []string{
		"3 + 4 * 5",
		"(3 + 4) * (5 - 2)",
		"10 / 2 + 3 * 4",
		"1 + 2 + 3 + 4",
	}

	for _, expr := range expressions {
		fmt.Printf("\nExpression: %s\n", expr)

		root, err := ParseExpression(expr)
		if err != nil {
			fmt.Printf("Error parsing expression: %v\n", err)
			continue
		}

		fmt.Printf("Parsed expression: %s\n", root.String())

		result, err := root.Evaluate()
		if err != nil {
			fmt.Printf("Error evaluating expression: %v\n", err)
			continue
		}
		fmt.Printf("Result: %.2f\n", result)
	}
}
