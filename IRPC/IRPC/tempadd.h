#ifndef TEMPADD_H
#define TEMPADD_H

int Add(int a, int b) {
	return a + b;
}
int Multiply(int a, int b) { return a* b; }
int FuncThing(int a, int b, int c) {
	return Add(a, b) + Multiply(b, c);
}

#endif